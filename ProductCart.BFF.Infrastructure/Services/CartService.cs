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
        var lab3Request = new Lab3AddItemRequest
        {
            ProductId = request.ProductId,
            Quantity = request.Quantity
        };

        return await _cartApiClient.AddItemToCartAsync(userId, userId, lab3Request);
    }

    public async Task<bool> RemoveItemFromCartAsync(string userId, string productId)
    {
        return await _cartApiClient.RemoveItemFromCartAsync(userId, productId);
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
}
