namespace PizzaShop.Data.Models;

public class Ingredient
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // e.g. "Meat", "Veggie", "Cheese"
}
