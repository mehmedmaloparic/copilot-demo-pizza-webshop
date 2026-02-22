using PizzaShop.Data.Models;

namespace PizzaShop.Api.Dtos;

public record OrderedPizzaRequest(
    bool IsCustom,
    int? PremadePizzaId,
    PizzaSize Size,
    int SauceId,
    List<int> IngredientIds);

public record PlaceOrderRequest(
    string CustomerName,
    string Street,
    string City,
    string PostalCode,
    string Phone,
    DateTime RequestedDeliveryTime,
    List<OrderedPizzaRequest> Pizzas);

public record OrderedPizzaDto(
    bool IsCustom,
    int? PremadePizzaId,
    string PizzaDisplayName,
    PizzaSize Size,
    string SauceName,
    List<string> IngredientNames,
    decimal Price);

public record OrderSummaryDto(
    string Id,
    DateTime PlacedAt,
    DateTime RequestedDeliveryTime,
    string CustomerName,
    string Street,
    string City,
    string PostalCode,
    string Phone,
    OrderStatus Status,
    decimal TotalPrice,
    int PizzaCount);

public record OrderDetailDto(
    string Id,
    DateTime PlacedAt,
    DateTime RequestedDeliveryTime,
    string CustomerName,
    string Street,
    string City,
    string PostalCode,
    string Phone,
    OrderStatus Status,
    decimal TotalPrice,
    List<OrderedPizzaDto> Pizzas,
    string? AdminNotes);

public record PlaceOrderResponse(string OrderId, decimal TotalPrice);

public record UpdateOrderStatusRequest(OrderStatus Status, string? AdminNotes);
