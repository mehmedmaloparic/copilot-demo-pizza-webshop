namespace PizzaShop.Data.Models;

public class PremadePizza
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SauceId { get; set; }
    public List<int> IngredientIds { get; set; } = new();
    public decimal BasePrice { get; set; }
}
