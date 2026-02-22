using System.Security.Cryptography;
using System.Text;
using PizzaShop.Api.Dtos;
using PizzaShop.Api.Services;
using PizzaShop.Data.Repositories;

namespace PizzaShop.Api.Endpoints;

public static class AdminEndpoints
{
    public static void MapAdminEndpoints(this WebApplication app)
    {
        app.MapPost("/api/admin/login", async (AdminLoginRequest request, IAdminRepository adminRepo, AdminTokenService tokenService) =>
        {
            var user = await adminRepo.GetAdminUserAsync(request.Username);
            if (user is null)
                return Results.Ok(new AdminLoginResponse(false, null));

            var hash = ComputeSha256(request.Password);
            if (!hash.Equals(user.PasswordHash, StringComparison.OrdinalIgnoreCase))
                return Results.Ok(new AdminLoginResponse(false, null));

            var token = tokenService.GenerateToken(user.Username);
            return Results.Ok(new AdminLoginResponse(true, token));
        });

        app.MapGet("/api/admin/orders", async (HttpContext ctx, IOrderRepository orderRepo, AdminTokenService tokenService) =>
        {
            if (!IsAuthorized(ctx, tokenService)) return Results.Unauthorized();

            var orders = await orderRepo.GetAllOrdersAsync();
            var dtos = orders
                .OrderByDescending(o => o.PlacedAt)
                .Select(o => new OrderSummaryDto(
                    o.Id,
                    o.PlacedAt,
                    o.RequestedDeliveryTime,
                    o.CustomerName,
                    o.Street,
                    o.City,
                    o.PostalCode,
                    o.Phone,
                    o.Status,
                    o.TotalPrice,
                    o.Pizzas.Count))
                .ToList();

            return Results.Ok(dtos);
        });

        app.MapGet("/api/admin/orders/{id}", async (string orderId, HttpContext ctx, IMenuRepository menuRepo, IOrderRepository orderRepo, AdminTokenService tokenService) =>
        {
            if (!IsAuthorized(ctx, tokenService)) return Results.Unauthorized();

            var order = await orderRepo.GetOrderByIdAsync(orderId);
            if (order is null) return Results.NotFound();

            return Results.Ok(await OrderEndpoints.MapToDetailDto(order, menuRepo));
        });

        app.MapPut("/api/admin/orders/{id}/status", async (string id, UpdateOrderStatusRequest request, HttpContext ctx, IOrderRepository orderRepo, AdminTokenService tokenService) =>
        {
            if (!IsAuthorized(ctx, tokenService)) return Results.Unauthorized();

            var order = await orderRepo.GetOrderByIdAsync(id);
            if (order is null) return Results.NotFound();

            order.Status = request.Status;
            if (request.AdminNotes is not null)
                order.AdminNotes = request.AdminNotes;

            await orderRepo.UpdateOrderAsync(order);
            return Results.Ok();
        });
    }

    private static bool IsAuthorized(HttpContext ctx, AdminTokenService tokenService)
    {
        var auth = ctx.Request.Headers.Authorization.ToString();
        if (auth.StartsWith("Bearer ", StringComparison.OrdinalIgnoreCase))
        {
            var token = auth["Bearer ".Length..].Trim();
            return tokenService.ValidateToken(token);
        }
        return false;
    }

    private static string ComputeSha256(string input)
    {
        var bytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
        return Convert.ToHexString(bytes).ToLowerInvariant();
    }
}
