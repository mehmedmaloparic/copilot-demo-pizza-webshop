using PizzaShop.Data.Models;
using PizzaShop.Data.Repositories;

namespace PizzaShop.Data;

public static class DataSeeder
{
    public static async Task SeedAsync(string dataDirectory)
    {
        Directory.CreateDirectory(dataDirectory);
        await SeedIngredientsAsync(dataDirectory);
        await SeedSaucesAsync(dataDirectory);
        await SeedPremadePizzasAsync(dataDirectory);
        await SeedAdminsAsync(dataDirectory);
        await SeedOrdersAsync(dataDirectory);
    }

    private static async Task SeedIngredientsAsync(string dir)
    {
        var path = Path.Combine(dir, "ingredients.json");
        if (File.Exists(path)) return;

        var ingredients = new List<Ingredient>
        {
            new() { Id = 1,  Name = "Mozzarella",       Category = "Cheese"  },
            new() { Id = 2,  Name = "Cheddar",          Category = "Cheese"  },
            new() { Id = 3,  Name = "Gorgonzola",       Category = "Cheese"  },
            new() { Id = 4,  Name = "Pepperoni",        Category = "Meat"    },
            new() { Id = 5,  Name = "Italian Sausage",  Category = "Meat"    },
            new() { Id = 6,  Name = "Bacon",            Category = "Meat"    },
            new() { Id = 7,  Name = "Chicken",          Category = "Meat"    },
            new() { Id = 8,  Name = "Mushrooms",        Category = "Veggie"  },
            new() { Id = 9,  Name = "Bell Peppers",     Category = "Veggie"  },
            new() { Id = 10, Name = "Red Onion",        Category = "Veggie"  },
            new() { Id = 11, Name = "Black Olives",     Category = "Veggie"  },
            new() { Id = 12, Name = "Spinach",          Category = "Veggie"  },
            new() { Id = 13, Name = "Tomatoes",         Category = "Veggie"  },
            new() { Id = 14, Name = "Jalapeños",        Category = "Veggie"  },
            new() { Id = 15, Name = "Pineapple",        Category = "Fruit"   },
            new() { Id = 16, Name = "Basil",            Category = "Herb"    }
        };

        await JsonFileHelper.WriteAsync(path, ingredients);
    }

    private static async Task SeedSaucesAsync(string dir)
    {
        var path = Path.Combine(dir, "sauces.json");
        if (File.Exists(path)) return;

        var sauces = new List<Sauce>
        {
            new() { Id = 1, Name = "Classic Tomato"  },
            new() { Id = 2, Name = "BBQ"             },
            new() { Id = 3, Name = "White Garlic"    },
            new() { Id = 4, Name = "Pesto"           },
            new() { Id = 5, Name = "Spicy Arrabbiata"}
        };

        await JsonFileHelper.WriteAsync(path, sauces);
    }

    private static async Task SeedPremadePizzasAsync(string dir)
    {
        var path = Path.Combine(dir, "premade_pizzas.json");
        if (File.Exists(path)) return;

        var pizzas = new List<PremadePizza>
        {
            new()
            {
                Id = 1,
                Name = "Margherita",
                Description = "The classic. Simple, perfect, timeless.",
                SauceId = 1,
                IngredientIds = new List<int> { 1, 16, 13 },
                BasePrice = 9.99m
            },
            new()
            {
                Id = 2,
                Name = "Pepperoni Bliss",
                Description = "Loaded with pepperoni because more is more.",
                SauceId = 1,
                IngredientIds = new List<int> { 1, 4 },
                BasePrice = 11.99m
            },
            new()
            {
                Id = 3,
                Name = "BBQ Chicken",
                Description = "Smoky BBQ sauce with tender chicken. A crowd favourite.",
                SauceId = 2,
                IngredientIds = new List<int> { 2, 7, 10 },
                BasePrice = 12.99m
            },
            new()
            {
                Id = 4,
                Name = "The Meat Lover",
                Description = "For those who believe vegetables are just decoration.",
                SauceId = 1,
                IngredientIds = new List<int> { 1, 4, 5, 6, 7 },
                BasePrice = 14.99m
            },
            new()
            {
                Id = 5,
                Name = "Garden Party",
                Description = "A veggie feast that even carnivores secretly enjoy.",
                SauceId = 4,
                IngredientIds = new List<int> { 1, 8, 9, 10, 12, 13 },
                BasePrice = 11.49m
            },
            new()
            {
                Id = 6,
                Name = "Hawaiian Controversy",
                Description = "Yes, it has pineapple. No, we won't apologise.",
                SauceId = 1,
                IngredientIds = new List<int> { 1, 6, 15 },
                BasePrice = 11.99m
            },
            new()
            {
                Id = 7,
                Name = "White Fire",
                Description = "Garlic sauce, gorgonzola, jalapeños. Not for the faint-hearted.",
                SauceId = 3,
                IngredientIds = new List<int> { 3, 14, 16 },
                BasePrice = 12.49m
            }
        };

        await JsonFileHelper.WriteAsync(path, pizzas);
    }

    private static async Task SeedAdminsAsync(string dir)
    {
        var path = Path.Combine(dir, "admins.json");
        if (File.Exists(path)) return;

        // Password: admin123  (SHA-256 hash)
        var admins = new List<AdminUser>
        {
            new()
            {
                Username = "admin",
                PasswordHash = "240be518fabd2724ddb6f04eeb1da5967448d7e831c08c8fa822809f74c720a9"
            }
        };

        await JsonFileHelper.WriteAsync(path, admins);
    }

    private static async Task SeedOrdersAsync(string dir)
    {
        var path = Path.Combine(dir, "orders.json");
        if (File.Exists(path)) return;

        await JsonFileHelper.WriteAsync(path, new List<Order>());
    }
}
