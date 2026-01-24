using ProductCart.BFF.Application.DTOs;
using ProductCart.BFF.Application.Services.Interfaces;
using ProductCart.BFF.Infrastructure.HttpClients;
using ProductCart.BFF.Infrastructure.Models.Lab3;

namespace ProductCart.BFF.Infrastructure.Services;

public class CartService : ICartService
{
    private readonly CartApiClient _cartApiClient;

    public CartService(CartApiClient cartApiClient)
    {
        _cartApiClient = cartApiClient;
    }

    public async Task<CartDto?> GetCartAsync(string userId)
    {
        var cart = await _cartApiClient.GetCartAsync(userId);

        if (cart == null)
            return null;

        return MapToCartDto(cart);
    }

    public async Task<bool> AddItemToCartAsync(string userId, AddToCartRequest request)
    {
        // Konwersja productId: jeśli to int, stwórz GUID
        var productIdGuid = ConvertToGuid(request.ProductId);

        var lab3Request = new Lab3AddItemRequest
        {
            ProductId = productIdGuid,
            Quantity = request.Quantity
        };

        return await _cartApiClient.AddItemToCartAsync(userId, userId, lab3Request);
    }

    public async Task<bool> RemoveItemFromCartAsync(string userId, string productId)
    {
        var productIdGuid = ConvertToGuid(productId);
        return await _cartApiClient.RemoveItemFromCartAsync(userId, productIdGuid);
    }

    public async Task<CheckoutResponse?> CheckoutCartAsync(string userId)
    {
        var result = await _cartApiClient.CheckoutCartAsync(userId, userId);

        if (result == null)
            return null;

        return new CheckoutResponse
        {
            CartId = result.CartId,
            Success = result.Success,
            Message = result.Message
        };
    }

    private static CartDto MapToCartDto(Lab3CartResponse cart)
    {
        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Status = cart.Status,
            LastActivityTime = cart.LastActivityTime,
            Items = cart.Items.Select(item => new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            }).ToList()
        };
    }

    private static string ConvertToGuid(string productId)
    {
        if (Guid.TryParse(productId, out _))
            return productId;

        if (int.TryParse(productId, out int numericId))
        {
            return $"00000000-0000-0000-0000-{numericId:D12}";
        }
        return productId;
    }
}
