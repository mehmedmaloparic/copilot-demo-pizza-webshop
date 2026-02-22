namespace PizzaShop.Data.Models;

public enum OrderStatus
{
    Pending,
    InProgress,
    Completed
}

public class Order
{
    public string Id { get; set; } = string.Empty;
    public DateTime PlacedAt { get; set; }
    public DateTime RequestedDeliveryTime { get; set; }

    // Customer info
    public string CustomerName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;

    public List<OrderedPizza> Pizzas { get; set; } = new();

    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public decimal TotalPrice { get; set; }

    public string? AdminNotes { get; set; }
}
