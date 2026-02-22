using PizzaShop.Data.Models;

namespace PizzaShop.Data.Repositories;

public class JsonMenuRepository : IMenuRepository
{
    private readonly string _dataDirectory;

    public JsonMenuRepository(string dataDirectory)
    {
        _dataDirectory = dataDirectory;
    }

    public Task<List<Ingredient>> GetIngredientsAsync() =>
        JsonFileHelper.ReadAsync<List<Ingredient>>(Path.Combine(_dataDirectory, "ingredients.json"));

    public Task<List<Sauce>> GetSaucesAsync() =>
        JsonFileHelper.ReadAsync<List<Sauce>>(Path.Combine(_dataDirectory, "sauces.json"));

    public Task<List<PremadePizza>> GetPremadePizzasAsync() =>
        JsonFileHelper.ReadAsync<List<PremadePizza>>(Path.Combine(_dataDirectory, "premade_pizzas.json"));
}
