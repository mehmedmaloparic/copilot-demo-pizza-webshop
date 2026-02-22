using Microsoft.AspNetCore.Mvc;
using PizzaShop.Web.Models;
using PizzaShop.Web.Services;

namespace PizzaShop.Web.Pages.Order;

public class CheckoutModel : PizzaPageModel
{
    private readonly PizzaApiClient _api;

    public CheckoutModel(PizzaApiClient api, CartService cartService) : base(cartService)
    {
        _api = api;
    }

    public List<CartItem> CartItems { get; set; } = new();
    public decimal Total { get; set; }

    [BindProperty] public string CustomerName { get; set; } = string.Empty;
    [BindProperty] public string Street { get; set; } = string.Empty;
    [BindProperty] public string City { get; set; } = string.Empty;
    [BindProperty] public string PostalCode { get; set; } = string.Empty;
    [BindProperty] public string Phone { get; set; } = string.Empty;
    [BindProperty] public DateTime RequestedDeliveryTime { get; set; } = DateTime.Now.AddHours(1);

    public string? ErrorMessage { get; set; }

    public void OnGet()
    {
        CartItems = CartService.GetCart();
        Total = CartItems.Sum(i => i.Price);
    }

    public IActionResult OnPostRemove(int index)
    {
        CartService.RemoveItem(index);
        return RedirectToPage();
    }

    public async Task<IActionResult> OnPostPlaceOrderAsync()
    {
        CartItems = CartService.GetCart();
        Total = CartItems.Sum(i => i.Price);

        if (CartItems.Count == 0)
        {
            ErrorMessage = "Your cart is empty!";
            return Page();
        }

        if (!ModelState.IsValid) return Page();

        var request = new PlaceOrderRequest
        {
            CustomerName = CustomerName,
            Street = Street,
            City = City,
            PostalCode = PostalCode,
            Phone = Phone,
            RequestedDeliveryTime = RequestedDeliveryTime,
            Pizzas = CartItems.Select(c => new OrderedPizzaRequest
            {
                IsCustom = c.IsCustom,
                PremadePizzaId = c.PremadePizzaId,
                Size = c.Size,
                SauceId = c.SauceId,
                IngredientIds = c.IngredientIds
            }).ToList()
        };

        try
        {
            var result = await _api.PlaceOrderAsync(request);
            CartService.ClearCart();
            return RedirectToPage("/Order/Confirmation", new { id = result!.OrderId });
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Could not place your order: {ex.Message}";
            return Page();
        }
    }
}
