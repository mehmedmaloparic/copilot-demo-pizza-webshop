using PizzaShop.Api.Endpoints;
using PizzaShop.Api.Services;
using PizzaShop.Data;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var dataDirectory = Path.Combine(builder.Environment.ContentRootPath, "data");
await DataSeeder.SeedAsync(dataDirectory);

builder.Services.AddPizzaShopData(dataDirectory);
builder.Services.AddSingleton(new AdminTokenService(
    builder.Configuration["AdminTokenSecret"] ?? "pizzashop-dev-secret-key"));

builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
        policy.WithOrigins(
                builder.Configuration["WebAppUrl"] ?? "https://localhost:7001")
            .AllowAnyMethod()
            .AllowAnyHeader());
});

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.PropertyNameCaseInsensitive = true;
});

var app = builder.Build();

app.UseCors();

app.MapMenuEndpoints();
app.MapOrderEndpoints();
app.MapAdminEndpoints();

app.Run();
