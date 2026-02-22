using PizzaShop.Data.Models;

namespace PizzaShop.Data.Repositories;

public interface IOrderRepository
{
    Task<List<Order>> GetAllOrdersAsync();
    Task<Order?> GetOrderByIdAsync(string id);
    Task SaveOrderAsync(Order order);
    Task UpdateOrderAsync(Order order);
}
