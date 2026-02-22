using PizzaShop.Data.Models;

namespace PizzaShop.Api.Services;

public static class PricingService
{
    private static readonly Dictionary<PizzaSize, decimal> SizeMultipliers = new()
    {
        { PizzaSize.Small,  0.80m },
        { PizzaSize.Medium, 1.00m },
        { PizzaSize.Large,  1.30m }
    };

    private const decimal CustomBasePricePerIngredient = 1.00m;
    private const decimal CustomBasePrice = 8.99m;

    public static decimal CalculatePremadePrice(decimal basePrice, PizzaSize size)
    {
        return Math.Round(basePrice * SizeMultipliers[size], 2);
    }

    public static decimal CalculateCustomPrice(int ingredientCount, PizzaSize size)
    {
        var total = CustomBasePrice + ingredientCount * CustomBasePricePerIngredient * SizeMultipliers[size];
        return Math.Round(total, 2);
    }
}
