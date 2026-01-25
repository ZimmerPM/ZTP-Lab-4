using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;
using ProductCart.MAUI.Helpers;
using System.Net.Http.Json;

namespace ProductCart.MAUI.Services;

public class ProductService : IProductService
{
    private readonly HttpClient _httpClient;
    private readonly DatabaseService _databaseService;

    public ProductService(HttpClient httpClient, DatabaseService databaseService)
    {
        _httpClient = httpClient;
        _databaseService = databaseService;
    }

    public async Task<List<Product>> GetProductsAsync()
    {
        try
        {
            Console.WriteLine("=== GetProductsAsync START ===");

            var isOnline = ConnectivityHelper.IsOnline();

            if (isOnline)
            {
                try
                {
                    var url = "http://localhost:5300/api/Products";
                    Console.WriteLine($"[ONLINE] Calling API: {url}");

                    var products = await _httpClient.GetFromJsonAsync<List<Product>>(url);

                    if (products != null && products.Count > 0)
                    {
                        Console.WriteLine($"[ONLINE] Received {products.Count} products from API");

                        var cachedProducts = products.Select(ProductCached.FromProduct).ToList();
                        await _databaseService.SaveProductsAsync(cachedProducts);

                        return products;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ONLINE] API call failed: {ex.Message}");
                    Console.WriteLine("[FALLBACK] Trying cache...");
                }
            }

            Console.WriteLine("[OFFLINE/FALLBACK] Loading from cache...");
            var cached = await _databaseService.GetAllProductsAsync();

            if (cached.Count > 0)
            {
                Console.WriteLine($"[CACHE] Loaded {cached.Count} products from cache");
                return cached.Select(c => c.ToProduct()).ToList();
            }

            Console.WriteLine("[ERROR] No products in cache and API unavailable");
            return new List<Product>();
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
            var isOnline = ConnectivityHelper.IsOnline();

            if (isOnline)
            {
                try
                {
                    var url = $"http://localhost:5300/api/Products/{id}";
                    Console.WriteLine($"[ONLINE] Calling API: {url}");

                    var product = await _httpClient.GetFromJsonAsync<Product>(url);

                    if (product != null)
                    {
                        Console.WriteLine($"[ONLINE] Product {id} fetched from API");
                        return product;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[ONLINE] API call failed: {ex.Message}");
                    Console.WriteLine("[FALLBACK] Trying cache...");
                }
            }

            Console.WriteLine($"[OFFLINE/FALLBACK] Loading product {id} from cache...");
            var cached = await _databaseService.GetProductByIdAsync(id);

            if (cached != null)
            {
                Console.WriteLine($"[CACHE] Product {id} loaded from cache");
                return cached.ToProduct();
            }

            Console.WriteLine($"[ERROR] Product {id} not found in cache");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR in GetProductByIdAsync: {ex.Message}");
            return null;
        }
    }
}
