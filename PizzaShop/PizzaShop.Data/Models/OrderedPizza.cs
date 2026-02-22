namespace PizzaShop.Data.Models;

public class OrderedPizza
{
    public bool IsCustom { get; set; }

    // Used when IsCustom = false
    public int? PremadePizzaId { get; set; }

    // Used for both premade (overrides) and custom
    public PizzaSize Size { get; set; } = PizzaSize.Medium;
    public int SauceId { get; set; }
    public List<int> IngredientIds { get; set; } = new();

    public string PizzaDisplayName { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
