namespace PizzaShop.Api.Dtos;

public record IngredientDto(int Id, string Name, string Category);

public record SauceDto(int Id, string Name);

public record PremadePizzaDto(
    int Id,
    string Name,
    string Description,
    int SauceId,
    string SauceName,
    List<IngredientDto> Ingredients,
    decimal BasePrice);

public record MenuDto(
    List<SauceDto> Sauces,
    List<IngredientDto> Ingredients,
    List<PremadePizzaDto> PremadePizzas);
