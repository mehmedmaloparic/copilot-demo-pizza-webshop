using PizzaShop.Data.Models;

namespace PizzaShop.Data.Repositories;

public interface IAdminRepository
{
    Task<AdminUser?> GetAdminUserAsync(string username);
}
