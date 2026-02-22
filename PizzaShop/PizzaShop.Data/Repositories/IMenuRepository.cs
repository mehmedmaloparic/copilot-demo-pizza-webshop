using PizzaShop.Data.Models;

namespace PizzaShop.Data.Repositories;

public interface IMenuRepository
{
    Task<List<Ingredient>> GetIngredientsAsync();
    Task<List<Sauce>> GetSaucesAsync();
    Task<List<PremadePizza>> GetPremadePizzasAsync();
}
