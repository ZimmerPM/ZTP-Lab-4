using ProductCart.BFF.Infrastructure.Models.Lab1;
using System.Net.Http.Json;

namespace ProductCart.BFF.Infrastructure.HttpClients;

public class ProductApiClient
{
    private readonly HttpClient _httpClient;

    public ProductApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Lab1ProductResponse>> GetProductsAsync()
    {
        var response = await _httpClient.GetAsync("/api/v1/products");
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadFromJsonAsync<List<Lab1ProductResponse>>()
               ?? new List<Lab1ProductResponse>();
    }

    public async Task<Lab1ProductDetailsResponse?> GetProductByIdAsync(int productId)
    {
        var response = await _httpClient.GetAsync($"/api/v1/products/{productId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<Lab1ProductDetailsResponse>();
    }
}
