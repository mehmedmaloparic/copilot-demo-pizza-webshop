using System.Text.Json;
using PizzaShop.Web.Models;

namespace PizzaShop.Web.Services;

public class CartService
{
    private const string SessionKey = "pizza_cart";
    private readonly IHttpContextAccessor _accessor;

    public CartService(IHttpContextAccessor accessor)
    {
        _accessor = accessor;
    }

    private ISession Session => _accessor.HttpContext!.Session;

    public List<CartItem> GetCart()
    {
        var json = Session.GetString(SessionKey);
        if (string.IsNullOrEmpty(json)) return new List<CartItem>();
        return JsonSerializer.Deserialize<List<CartItem>>(json) ?? new List<CartItem>();
    }

    public void SaveCart(List<CartItem> cart)
    {
        Session.SetString(SessionKey, JsonSerializer.Serialize(cart));
    }

    public void AddItem(CartItem item)
    {
        var cart = GetCart();
        cart.Add(item);
        SaveCart(cart);
    }

    public void RemoveItem(int index)
    {
        var cart = GetCart();
        if (index >= 0 && index < cart.Count)
        {
            cart.RemoveAt(index);
            SaveCart(cart);
        }
    }

    public void ClearCart()
    {
        Session.Remove(SessionKey);
    }

    public int Count => GetCart().Count;
}
