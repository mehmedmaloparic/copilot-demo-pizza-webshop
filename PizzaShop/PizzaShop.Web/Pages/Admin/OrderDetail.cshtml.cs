using Microsoft.AspNetCore.Mvc;
using PizzaShop.Web.Models;
using PizzaShop.Web.Services;

namespace PizzaShop.Web.Pages.Admin;

public class OrderDetailPageModel : PizzaPageModel
{
    private readonly PizzaApiClient _api;

    public OrderDetailPageModel(PizzaApiClient api, CartService cartService) : base(cartService)
    {
        _api = api;
    }

    public Models.OrderDetailModel? Order { get; set; }
    public string? ErrorMessage { get; set; }
    public string? SuccessMessage { get; set; }

    [BindProperty] public OrderStatus NewStatus { get; set; }
    [BindProperty] public string? AdminNotes { get; set; }

    public async Task<IActionResult> OnGetAsync(string id)
    {
        var token = HttpContext.Session.GetString("admin_token");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Admin/Login");

        try
        {
            Order = await _api.GetAdminOrderAsync(id, token);
            if (Order != null)
            {
                NewStatus = Order.Status;
                AdminNotes = Order.AdminNotes;
            }
        }
        catch
        {
            ErrorMessage = "Could not load order.";
        }

        return Page();
    }

    public async Task<IActionResult> OnPostUpdateAsync(string id)
    {
        var token = HttpContext.Session.GetString("admin_token");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Admin/Login");

        try
        {
            await _api.UpdateOrderStatusAsync(id, NewStatus, AdminNotes, token);
            SuccessMessage = "Order updated successfully.";
            Order = await _api.GetAdminOrderAsync(id, token);
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Update failed: {ex.Message}";
            Order = await _api.GetAdminOrderAsync(id, token);
        }

        return Page();
    }
}
