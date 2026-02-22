using System.Net.Http.Json;
using System.Text.Json;
using System.Text.Json.Serialization;
using PizzaShop.Web.Models;

namespace PizzaShop.Web.Services;

public class PizzaApiClient
{
    private readonly HttpClient _http;

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        Converters = { new JsonStringEnumConverter() }
    };

    public PizzaApiClient(HttpClient http)
    {
        _http = http;
    }

    // ?? Menu ????????????????????????????????????????????????????????????????

    public async Task<MenuModel?> GetMenuAsync()
    {
        return await _http.GetFromJsonAsync<MenuModel>("/api/menu", JsonOptions);
    }

    // ?? Orders (customer) ???????????????????????????????????????????????????

    public async Task<PlaceOrderResponse?> PlaceOrderAsync(PlaceOrderRequest request)
    {
        var response = await _http.PostAsJsonAsync("/api/orders", request, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<PlaceOrderResponse>(JsonOptions);
    }

    public async Task<OrderDetailModel?> GetOrderAsync(string id)
    {
        return await _http.GetFromJsonAsync<OrderDetailModel>($"/api/orders/{id}", JsonOptions);
    }

    // ?? Admin ????????????????????????????????????????????????????????????????

    public async Task<AdminLoginResponse?> AdminLoginAsync(string username, string password)
    {
        var response = await _http.PostAsJsonAsync("/api/admin/login",
            new { username, password }, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<AdminLoginResponse>(JsonOptions);
    }

    public async Task<List<OrderSummaryModel>?> GetAdminOrdersAsync(string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/admin/orders");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<List<OrderSummaryModel>>(JsonOptions);
    }

    public async Task<OrderDetailModel?> GetAdminOrderAsync(string id, string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/admin/orders/{id}");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<OrderDetailModel>(JsonOptions);
    }

    public async Task UpdateOrderStatusAsync(string id, OrderStatus status, string? adminNotes, string token)
    {
        using var request = new HttpRequestMessage(HttpMethod.Put, $"/api/admin/orders/{id}/status");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);
        request.Content = JsonContent.Create(new { status, adminNotes }, options: JsonOptions);
        var response = await _http.SendAsync(request);
        response.EnsureSuccessStatusCode();
    }
}

// ?? Request/response models used by PizzaApiClient ??????????????????????????

public class PlaceOrderRequest
{
    public string CustomerName { get; set; } = string.Empty;
    public string Street { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string PostalCode { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public DateTime RequestedDeliveryTime { get; set; }
    public List<OrderedPizzaRequest> Pizzas { get; set; } = new();
}

public class OrderedPizzaRequest
{
    public bool IsCustom { get; set; }
    public int? PremadePizzaId { get; set; }
    public PizzaSize Size { get; set; }
    public int SauceId { get; set; }
    public List<int> IngredientIds { get; set; } = new();
}

public class PlaceOrderResponse
{
    public string OrderId { get; set; } = string.Empty;
    public decimal TotalPrice { get; set; }
}

public class AdminLoginResponse
{
    public bool Success { get; set; }
    public string? Token { get; set; }
}
