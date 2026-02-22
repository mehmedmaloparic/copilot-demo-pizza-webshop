using PizzaShop.Api.Dtos;
using PizzaShop.Data.Repositories;

namespace PizzaShop.Api.Endpoints;

public static class MenuEndpoints
{
    public static void MapMenuEndpoints(this WebApplication app)
    {
        app.MapGet("/api/menu", async (IMenuRepository menuRepo) =>
        {
            var sauces = await menuRepo.GetSaucesAsync();
            var ingredients = menuRepo.GetIngredientsAsync();
            var premade = await menuRepo.GetPremadePizzasAsync();

            var sauceDtos = sauces.Select(s => new SauceDto(s.Id, s.Name)).ToList();
            var ingredientDtos = (await ingredients).Select(i => new IngredientDto(i.Id, i.Name, i.Category)).ToList();
            var ingredientMap = ingredientDtos.ToDictionary(i => i.Id);
            var sauceMap = sauceDtos.ToDictionary(s => s.Id);

            var premadeDtos = premade.Select(p => new PremadePizzaDto(
                p.Id,
                p.Name,
                p.Description,
                p.SauceId,
                sauceMap.TryGetValue(p.SauceId, out var sauce) ? sauce.Name : "Unknown",
                p.IngredientIds
                    .Where(id => ingredientMap.ContainsKey(id))
                    .Select(id => ingredientMap[id])
                    .ToList(),
                p.BasePrice
            )).ToList();

            return Results.Ok(new MenuDto(sauceDtos, ingredientDtos, premadeDtos));
        });
    }
}
