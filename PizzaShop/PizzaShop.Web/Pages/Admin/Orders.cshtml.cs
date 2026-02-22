using Microsoft.AspNetCore.Mvc;
using PizzaShop.Web.Models;
using PizzaShop.Web.Services;

namespace PizzaShop.Web.Pages.Admin;

public class OrdersModel : PizzaPageModel
{
    private readonly PizzaApiClient _api;

    public OrdersModel(PizzaApiClient api, CartService cartService) : base(cartService)
    {
        _api = api;
    }

    public List<OrderSummaryModel> Orders { get; set; } = new();
    public string? ErrorMessage { get; set; }

    public async Task<IActionResult> OnGetAsync()
    {
        var token = HttpContext.Session.GetString("admin_token");
        if (string.IsNullOrEmpty(token))
            return RedirectToPage("/Admin/Login");

        try
        {
            Orders = await _api.GetAdminOrdersAsync(token) ?? new List<OrderSummaryModel>();
            Orders = Orders.OrderBy(o => o.PlacedAt).ToList();
        }
        catch
        {
            ErrorMessage = "Could not load orders. Check that the API is running.";
        }

        return Page();
    }

    public IActionResult OnPostLogout()
    {
        HttpContext.Session.Remove("admin_token");
        return RedirectToPage("/Admin/Login");
    }
}
