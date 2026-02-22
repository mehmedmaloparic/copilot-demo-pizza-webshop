namespace PizzaShop.Api.Dtos;

public record AdminLoginRequest(string Username, string Password);
public record AdminLoginResponse(bool Success, string? Token);
