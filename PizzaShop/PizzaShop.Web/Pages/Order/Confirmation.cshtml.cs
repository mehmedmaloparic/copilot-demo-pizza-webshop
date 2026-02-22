using PizzaShop.Web.Models;
using PizzaShop.Web.Services;

namespace PizzaShop.Web.Pages.Order;

public class ConfirmationModel : PizzaPageModel
{
    private readonly PizzaApiClient _api;

    public ConfirmationModel(PizzaApiClient api, CartService cartService) : base(cartService)
    {
        _api = api;
    }

    public OrderDetailModel? Order { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync(string id)
    {
        try
        {
            Order = await _api.GetOrderAsync(id);
        }
        catch
        {
            ErrorMessage = "We couldn't load your order details, but your order was placed!";
        }
    }
}
