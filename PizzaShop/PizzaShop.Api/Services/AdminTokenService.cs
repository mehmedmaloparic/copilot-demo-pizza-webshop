using System.Security.Cryptography;
using System.Text;

namespace PizzaShop.Api.Services;

public class AdminTokenService
{
    private readonly HashSet<string> _validTokens = new();
    private readonly string _secret;

    public AdminTokenService(string secret)
    {
        _secret = secret;
    }

    public string GenerateToken(string username)
    {
        var raw = $"{username}:{DateTime.UtcNow.Ticks}:{Guid.NewGuid()}";
        var token = ComputeHmac(raw);
        _validTokens.Add(token);
        return token;
    }

    public bool ValidateToken(string? token)
    {
        if (string.IsNullOrWhiteSpace(token)) return false;
        return _validTokens.Contains(token);
    }

    public void RevokeToken(string token) => _validTokens.Remove(token);

    private string ComputeHmac(string data)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_secret));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
