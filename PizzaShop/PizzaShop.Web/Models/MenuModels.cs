namespace PizzaShop.Web.Models;

public class IngredientModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
}

public class SauceModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
}

public class PremadePizzaModel
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int SauceId { get; set; }
    public string SauceName { get; set; } = string.Empty;
    public List<IngredientModel> Ingredients { get; set; } = new();
    public decimal BasePrice { get; set; }
}

public class MenuModel
{
    public List<SauceModel> Sauces { get; set; } = new();
    public List<IngredientModel> Ingredients { get; set; } = new();
    public List<PremadePizzaModel> PremadePizzas { get; set; } = new();
}
