using ProductCart.MAUI.Models;
using ProductCart.MAUI.Services.Interfaces;
using System.Net.Http.Json;

namespace ProductCart.MAUI.Services;

public class CartService : ICartService
{
    private readonly HttpClient _httpClient;
    private const string BaseUrl = "http://localhost:5200";

    private readonly Guid _userId = Guid.Parse("11111111-1111-1111-1111-111111111111");
    private Guid _currentCartId;

    public CartService(HttpClient httpClient)
    {
        _httpClient = httpClient;

        var savedCartId = Preferences.Get("CurrentCartId", string.Empty);
        if (string.IsNullOrEmpty(savedCartId) || !Guid.TryParse(savedCartId, out _currentCartId))
        {
            _currentCartId = Guid.NewGuid();
            Preferences.Set("CurrentCartId", _currentCartId.ToString());
        }
        Console.WriteLine($"CartService initialized with CartId: {_currentCartId}");
    }

    public async Task<Cart?> GetCartAsync(string userId)
    {
        try
        {
            Console.WriteLine($"=== GetCartAsync for cartId: {_currentCartId} ===");

            var response = await _httpClient.GetAsync($"{BaseUrl}/carts/{_currentCartId}");

            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                return new Cart
                {
                    Id = 1,
                    UserId = userId,
                    Items = new List<CartItem>()
                };
            }

            response.EnsureSuccessStatusCode();
            var result = await response.Content.ReadFromJsonAsync<GetCartResponseDto>();

            if (result == null) return null;

            var cart = new Cart
            {
                Id = 1,
                UserId = userId,
                Items = result.Items.Select(i => new CartItem
                {
                    ProductId = int.Parse(i.ProductId.ToString().Substring(0, 8), System.Globalization.NumberStyles.HexNumber),
                    ProductName = i.ProductName ?? "Unknown",
                    Price = i.UnitPrice,
                    Quantity = i.Quantity
                }).ToList()
            };

            return cart;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR getting cart: {ex.Message}");
            return null;
        }
    }

    public async Task<bool> AddProductToCartAsync(string userId, Guid productId, int quantity)
    {
        try
        {
            Console.WriteLine($"=== AddProductToCartAsync ===");
            Console.WriteLine($"CartId: {_currentCartId}, UserId: {_userId}, ProductId: {productId}, Quantity: {quantity}");

            var request = new { ProductId = productId, Quantity = quantity };

            var response = await _httpClient.PostAsJsonAsync(
                $"{BaseUrl}/carts/{_currentCartId}/items?userId={_userId}",
                request);

            var responseContent = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Response: {response.StatusCode} - {responseContent}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR adding product to cart: {ex.Message}");
            return false;
        }
    }

    public async Task<bool> RemoveProductFromCartAsync(string userId, Guid productId)
    {
        try
        {
            Console.WriteLine($"=== RemoveProductFromCartAsync ===");
            Console.WriteLine($"CartId: {_currentCartId}, UserId: {_userId}, ProductId: {productId}");

            var response = await _httpClient.DeleteAsync(
                $"{BaseUrl}/carts/{_currentCartId}/items/{productId}?userId={_userId}");

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR removing product from cart: {ex.Message}");
            return false;
        }
    }

    public async Task<Cart?> CheckoutCartAsync(string userId)
    {
        try
        {
            Console.WriteLine($"=== CheckoutCartAsync ===");

            var response = await _httpClient.PostAsync(
                $"{BaseUrl}/carts/{_currentCartId}/checkout?userId={_userId}",
                null);

            if (!response.IsSuccessStatusCode) return null;

            _currentCartId = Guid.NewGuid();
            Preferences.Set("CurrentCartId", _currentCartId.ToString());
            Console.WriteLine($"Checkout successful - new CartId: {_currentCartId}");

            return new Cart
            {
                Id = 1,
                UserId = userId,
                Items = new List<CartItem>()
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine($"ERROR checking out cart: {ex.Message}");
            return null;
        }
    }
}

public class GetCartResponseDto
{
    public Guid CartId { get; set; }
    public Guid UserId { get; set; }
    public List<CartItemResponseDto> Items { get; set; } = new();
    public decimal TotalAmount { get; set; }
}

public class CartItemResponseDto
{
    public Guid ProductId { get; set; }
    public string? ProductName { get; set; }
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
