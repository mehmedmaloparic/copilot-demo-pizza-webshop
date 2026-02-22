using Microsoft.Extensions.DependencyInjection;
using PizzaShop.Data.Repositories;

namespace PizzaShop.Data;

public static class DataServiceExtensions
{
    public static IServiceCollection AddPizzaShopData(this IServiceCollection services, string dataDirectory)
    {
        services.AddSingleton<IMenuRepository>(_ => new JsonMenuRepository(dataDirectory));
        services.AddSingleton<IOrderRepository>(_ => new JsonOrderRepository(dataDirectory));
        services.AddSingleton<IAdminRepository>(_ => new JsonAdminRepository(dataDirectory));
        return services;
    }
}
