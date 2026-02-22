namespace PizzaShop.Web.Models;

public enum PizzaSize { Small, Medium, Large }

public enum OrderStatus { Pending, InProgress, Completed }

public class CartItem
{
    public bool IsCustom { get; set; }
    public int? PremadePizzaId { get; set; }
    public string DisplayName { get; set; } = string.Empty;
    public PizzaSize Size { get; set; } = PizzaSize.Medium;
    public int SauceId { get; set; }
    public string SauceName { get; set; } = string.Empty;
    public List<int> IngredientIds { get; set; } = new();
    public List<string> IngredientNames { get; set; } = new();
    public decimal Price { get; set; }
}

public class OrderedPizzaModel
{
    public bool IsCustom { get; set; }
    public int? PremadePizzaId { get; set; }
    public string PizzaDisplayName { get; set; } = string.Empty;
    public PizzaSize Size { get; set; }
    public string SauceName { get; set; } = string.Empty;
    public List<string> IngredientNames { get; set; } = new();
    public decimal Price { get; set; }
}

public class OrderSummaryModel
{
    public string Id { get; set; } = string.Empty;
    public DateTime PlacedAt { get; set; }
    public DateTime RequestedDeliveryTime { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public int PizzaCount { get; set; }
}

public class OrderDetailModel
{
    public string Id { get; set; } = string.Empty;
    public DateTime PlacedAt { get; set; }
    public DateTime RequestedDeliveryTime { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public OrderStatus Status { get; set; }
    public decimal TotalPrice { get; set; }
    public List<OrderedPizzaModel> Pizzas { get; set; } = new();
    public string? AdminNotes { get; set; }
}
