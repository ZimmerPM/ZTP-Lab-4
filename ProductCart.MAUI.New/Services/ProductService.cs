using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;
using System.Dynamic;
using System.Net.Http.Json;

namespace ProductCart.MAUI.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;

    public ProductService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            Console.WriteLine("=== GetProductsAsync START ===");
            var url = "http://localhost:5300/api/Products";
            Console.WriteLine($"Calling URL: {url}");

            var products = await _httpClient.GetFromJsonAsync<List<Product>>(url);

            Console.WriteLine($"Received {products?.Count ?? 0} products");
            return products ?? new List<Product>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in GetProductsAsync: {ex.Message}");
            return new List<Product>();
        }
    }

    public async Task<Product?> GetProductByIdAsync(int id)
    {
        try
        {
            var url = $"http://localhost:5300/api/Products/{id}";
            Console.WriteLine($"Calling URL: {url}");
            return await _httpClient.GetFromJsonAsync<Product>(url);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in GetProductByIdAsync: {ex.Message}");
            return null;
        }
    }
}
