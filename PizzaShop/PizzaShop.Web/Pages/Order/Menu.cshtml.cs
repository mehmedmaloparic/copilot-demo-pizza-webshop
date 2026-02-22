using Microsoft.AspNetCore.Mvc;
using PizzaShop.Web.Models;
using PizzaShop.Web.Services;

namespace PizzaShop.Web.Pages.Order;

public class MenuPageModel : PizzaPageModel
{
    private readonly PizzaApiClient _api;

    public MenuPageModel(PizzaApiClient api, CartService cartService) : base(cartService)
    {
        _api = api;
    }

    public MenuModel? Menu { get; set; }
    public string? ErrorMessage { get; set; }

    public async Task OnGetAsync()
    {
        try
        {
            Menu = await _api.GetMenuAsync();
        }
        catch
        {
            ErrorMessage = "Could not load the menu. Make sure the API is running.";
        }
    }

    // Add a premade pizza to the cart
    public async Task<IActionResult> OnPostAddPremadeAsync(int pizzaId, PizzaSize size)
    {
        var menu = await _api.GetMenuAsync();
        var pizza = menu?.PremadePizzas.FirstOrDefault(p => p.Id == pizzaId);
        if (pizza == null) return RedirectToPage();

        var price = CalculatePremadePrice(pizza.BasePrice, size);

        CartService.AddItem(new CartItem
        {
            IsCustom = false,
            PremadePizzaId = pizzaId,
            DisplayName = pizza.Name,
            Size = size,
            SauceId = pizza.SauceId,
            SauceName = pizza.SauceName,
            IngredientIds = pizza.Ingredients.Select(i => i.Id).ToList(),
            IngredientNames = pizza.Ingredients.Select(i => i.Name).ToList(),
            Price = price
        });

        return RedirectToPage("/Order/Checkout");
    }

    // Add a custom pizza to the cart
    public async Task<IActionResult> OnPostAddCustomAsync(
        PizzaSize size,
        int sauceId,
        [FromForm] List<int> ingredientIds)
    {
        var menu = await _api.GetMenuAsync();
        if (menu == null) return RedirectToPage();

        var sauce = menu.Sauces.FirstOrDefault(s => s.Id == sauceId);
        var selectedIngredients = menu.Ingredients.Where(i => ingredientIds.Contains(i.Id)).ToList();

        var price = CalculateCustomPrice(selectedIngredients.Count, size);

        CartService.AddItem(new CartItem
        {
            IsCustom = true,
            PremadePizzaId = null,
            DisplayName = "Custom Pizza",
            Size = size,
            SauceId = sauceId,
            SauceName = sauce?.Name ?? "Unknown",
            IngredientIds = ingredientIds,
            IngredientNames = selectedIngredients.Select(i => i.Name).ToList(),
            Price = price
        });

        return RedirectToPage("/Order/Checkout");
    }

    private static decimal CalculatePremadePrice(decimal basePrice, PizzaSize size) =>
        Math.Round(basePrice * SizeMultiplier(size), 2);

    private static decimal CalculateCustomPrice(int ingredientCount, PizzaSize size) =>
        Math.Round((8.99m + ingredientCount * 1.00m) * SizeMultiplier(size), 2);

    private static decimal SizeMultiplier(PizzaSize size) => size switch
    {
        PizzaSize.Small => 0.80m,
        PizzaSize.Large => 1.30m,
        _ => 1.00m
    };
}
