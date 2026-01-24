using ProductCart.BFF.Infrastructure.Models.Lab3;
using System.Net.Http.Json;

namespace ProductCart.BFF.Infrastructure.HttpClients;

public class CartApiClient
{
    private readonly HttpClient _httpClient;

    public CartApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<Lab3CartResponse?> GetCartAsync(string cartId)
    {
        var response = await _httpClient.GetAsync($"/carts/{cartId}");

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<Lab3CartResponse>();
    }

    public async Task<bool> AddItemToCartAsync(string cartId, string userId, Lab3AddItemRequest request)
    {
        var response = await _httpClient.PostAsJsonAsync(
            $"/carts/{cartId}/items?userId={userId}",
            request);

        return response.IsSuccessStatusCode;
    }

    public async Task<bool> RemoveItemFromCartAsync(string cartId, string productId)
    {
        var response = await _httpClient.DeleteAsync($"/carts/{cartId}/items/{productId}");

        return response.IsSuccessStatusCode;
    }

    public async Task<Lab3CheckoutResponse?> CheckoutCartAsync(string cartId, string userId)
    {
        var response = await _httpClient.PostAsync(
            $"/carts/{cartId}/checkout?userId={userId}",
            null);

        if (!response.IsSuccessStatusCode)
            return null;

        return await response.Content.ReadFromJsonAsync<Lab3CheckoutResponse>();
    }
}
