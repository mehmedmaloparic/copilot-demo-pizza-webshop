using System.Text.Json;

namespace PizzaShop.Data.Repositories;

internal static class JsonFileHelper
{
    private static readonly JsonSerializerOptions Options = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public static async Task<T> ReadAsync<T>(string filePath) where T : new()
    {
        if (!File.Exists(filePath))
            return new T();

        await using var stream = File.OpenRead(filePath);
        return await JsonSerializer.DeserializeAsync<T>(stream, Options) ?? new T();
    }

    public static async Task WriteAsync<T>(string filePath, T data)
    {
        var dir = Path.GetDirectoryName(filePath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);

        await using var stream = File.Create(filePath);
        await JsonSerializer.SerializeAsync(stream, data, Options);
    }
}
