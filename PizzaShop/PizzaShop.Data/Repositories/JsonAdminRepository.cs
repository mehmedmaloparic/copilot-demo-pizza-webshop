using PizzaShop.Data.Models;

namespace PizzaShop.Data.Repositories;

public class JsonAdminRepository : IAdminRepository
{
    private readonly string _filePath;

    public JsonAdminRepository(string dataDirectory)
    {
        _filePath = Path.Combine(dataDirectory, "admins.json");
    }

    public async Task<AdminUser?> GetAdminUserAsync(string username)
    {
        var users = await JsonFileHelper.ReadAsync<List<AdminUser>>(_filePath);
        return users.FirstOrDefault(u => u.Username.Equals(username, StringComparison.OrdinalIgnoreCase));
    }
}
