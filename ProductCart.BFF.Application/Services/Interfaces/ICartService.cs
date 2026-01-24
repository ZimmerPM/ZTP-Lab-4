using ProductCart.BFF.Application.DTOs;

namespace ProductCart.BFF.Application.Services.Interfaces;

public interface ICartService
{
    Task<CartDto?> GetCartAsync(string userId);
    Task<bool> AddItemToCartAsync(string userId, AddToCartRequest request);
    Task<bool> RemoveItemFromCartAsync(string userId, string productId);
    Task<CheckoutResponse?> CheckoutCartAsync(string userId);
}
