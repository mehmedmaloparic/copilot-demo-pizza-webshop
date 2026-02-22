using PizzaShop.Data.Models;

namespace PizzaShop.Data.Repositories;

public class JsonOrderRepository : IOrderRepository
{
    private readonly string _filePath;
    private readonly SemaphoreSlim _lock = new(1, 1);

    public JsonOrderRepository(string dataDirectory)
    {
        _filePath = Path.Combine(dataDirectory, "orders.json");
    }

    public Task<List<Order>> GetAllOrdersAsync() =>
        JsonFileHelper.ReadAsync<List<Order>>(_filePath);

    public async Task<Order?> GetOrderByIdAsync(string id)
    {
        var orders = await GetAllOrdersAsync();
        return orders.FirstOrDefault(o => o.Id == id);
    }

    public async Task SaveOrderAsync(Order order)
    {
        await _lock.WaitAsync();
        try
        {
            var orders = await GetAllOrdersAsync();
            orders.Add(order);
            await JsonFileHelper.WriteAsync(_filePath, orders);
        }
        finally
        {
            _lock.Release();
        }
    }

    public async Task UpdateOrderAsync(Order order)
    {
        await _lock.WaitAsync();
        try
        {
            var orders = await GetAllOrdersAsync();
            var index = orders.FindIndex(o => o.Id == order.Id);
            if (index >= 0)
                orders[index] = order;
            await JsonFileHelper.WriteAsync(_filePath, orders);
        }
        finally
        {
            _lock.Release();
        }
    }
}
