using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;
using ProductCart.MAUI.Helpers;
using System.Net.Http.Json;
using System.Diagnostics;

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
            Debug.WriteLine("=== GetProductsAsync START ===");

            var isOnline = ConnectivityHelper.IsOnline();

            if (isOnline)
            {
                // ONLINE
                try
                {
                    var url = "http://localhost:5300/api/Products";
                    Debug.WriteLine($"[ONLINE] Calling API: {url}");

                    var products = await _httpClient.GetFromJsonAsync<List<Product>>(url);

                    if (products != null && products.Count > 0)
                    {
                        Debug.WriteLine($"[ONLINE] Received {products.Count} products from API");

                        var cachedProducts = products.Select(ProductCached.FromProduct).ToList();
                        await _databaseService.SaveProductsAsync(cachedProducts);

                        return products;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ONLINE] API call failed: {ex.Message}");
                    Debug.WriteLine("[FALLBACK] Trying cache...");
                }
            }

            // OFFLINE
            Debug.WriteLine("[OFFLINE/FALLBACK] Loading from cache...");
            var cached = await _databaseService.GetAllProductsAsync();

            if (cached.Count > 0)
            {
                Debug.WriteLine($"[CACHE] Loaded {cached.Count} products from cache");
                return cached.Select(c => c.ToProduct()).ToList();
            }

            Debug.WriteLine("[ERROR] No products in cache and API unavailable");
            return new List<Product>();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR in GetProductsAsync: {ex.Message}");
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
                // ONLINE
                try
                {
                    var url = $"http://localhost:5300/api/Products/{id}";
                    Debug.WriteLine($"[ONLINE] Calling API: {url}");

                    var product = await _httpClient.GetFromJsonAsync<Product>(url);

                    if (product != null)
                    {
                        Debug.WriteLine($"[ONLINE] Product {id} fetched from API");
                        return product;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"[ONLINE] API call failed: {ex.Message}");
                    Debug.WriteLine("[FALLBACK] Trying cache...");
                }
            }

            // OFFLINE
            Debug.WriteLine($"[OFFLINE/FALLBACK] Loading product {id} from cache...");
            var cached = await _databaseService.GetProductByIdAsync(id);

            if (cached != null)
            {
                Debug.WriteLine($"[CACHE] Product {id} loaded from cache");
                return cached.ToProduct();
            }

            Debug.WriteLine($"[ERROR] Product {id} not found in cache");
            return null;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"ERROR in GetProductByIdAsync: {ex.Message}");
            return null;
        }
    }
}