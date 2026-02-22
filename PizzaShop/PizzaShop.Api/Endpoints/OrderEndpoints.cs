using PizzaShop.Api.Dtos;
using PizzaShop.Api.Services;
using PizzaShop.Data.Models;
using PizzaShop.Data.Repositories;

namespace PizzaShop.Api.Endpoints;

public static class OrderEndpoints
{
    public static void MapOrderEndpoints(this WebApplication app)
    {
        app.MapPost("/api/orders", async (PlaceOrderRequest request, IMenuRepository menuRepo, IOrderRepository orderRepo) =>
        {
            if (request.Pizzas == null || request.Pizzas.Count == 0)
                return Results.BadRequest("At least one pizza is required.");

            var sauces = await menuRepo.GetSaucesAsync();
            var ingredients = await menuRepo.GetIngredientsAsync();
            var premade = await menuRepo.GetPremadePizzasAsync();

            var sauceMap = sauces.ToDictionary(s => s.Id);
            var ingredientMap = ingredients.ToDictionary(i => i.Id);
            var premadeMap = premade.ToDictionary(p => p.Id);

            var orderedPizzas = new List<OrderedPizza>();

            foreach (var req in request.Pizzas)
            {
                string displayName;
                decimal price;
                int sauceId = req.SauceId;
                List<int> ingredientIds = req.IngredientIds ?? new List<int>();

                if (!req.IsCustom && req.PremadePizzaId.HasValue && premadeMap.TryGetValue(req.PremadePizzaId.Value, out var template))
                {
                    displayName = template.Name;
                    price = PricingService.CalculatePremadePrice(template.BasePrice, req.Size);
                    sauceId = template.SauceId;
                    ingredientIds = template.IngredientIds;
                }
                else
                {
                    displayName = "Custom Pizza";
                    price = PricingService.CalculateCustomPrice(ingredientIds.Count, req.Size);
                }

                orderedPizzas.Add(new OrderedPizza
                {
                    IsCustom = req.IsCustom,
                    PremadePizzaId = req.PremadePizzaId,
                    Size = req.Size,
                    SauceId = sauceId,
                    IngredientIds = ingredientIds,
                    PizzaDisplayName = displayName,
                    Price = price
                });
            }

            var total = orderedPizzas.Sum(p => p.Price);

            var order = new Order
            {
                Id = Guid.NewGuid().ToString("N")[..8].ToUpper(),
                PlacedAt = DateTime.UtcNow,
                RequestedDeliveryTime = request.RequestedDeliveryTime,
                CustomerName = request.CustomerName,
                Street = request.Street,
                City = request.City,
                PostalCode = request.PostalCode,
                Phone = request.Phone,
                Pizzas = orderedPizzas,
                Status = OrderStatus.Pending,
                TotalPrice = total
            };

            await orderRepo.SaveOrderAsync(order);

            return Results.Ok(new PlaceOrderResponse(order.Id, order.TotalPrice));
        });

        app.MapGet("/api/orders/{id}", async (string id, IMenuRepository menuRepo, IOrderRepository orderRepo) =>
        {
            var order = await orderRepo.GetOrderByIdAsync(id);
            if (order is null) return Results.NotFound();

            return Results.Ok(await MapToDetailDto(order, menuRepo));
        });
    }

    internal static async Task<OrderDetailDto> MapToDetailDto(Order order, IMenuRepository menuRepo)
    {
        var sauces = await menuRepo.GetSaucesAsync();
        var ingredients = await menuRepo.GetIngredientsAsync();
        var sauceMap = sauces.ToDictionary(s => s.Id, s => s.Name);
        var ingredientMap = ingredients.ToDictionary(i => i.Id, i => i.Name);

        var pizzaDtos = order.Pizzas.Select(p => new OrderedPizzaDto(
            p.IsCustom,
            p.PremadePizzaId,
            p.PizzaDisplayName,
            p.Size,
            sauceMap.TryGetValue(p.SauceId, out var sn) ? sn : "Unknown",
            p.IngredientIds.Where(ingredientMap.ContainsKey).Select(id => ingredientMap[id]).ToList(),
            p.Price
        )).ToList();

        return new OrderDetailDto(
            order.Id,
            order.PlacedAt,
            order.RequestedDeliveryTime,
            order.CustomerName,
            order.Street,
            order.City,
            order.PostalCode,
            order.Phone,
            order.Status,
            order.TotalPrice,
            pizzaDtos,
            order.AdminNotes);
    }
}
