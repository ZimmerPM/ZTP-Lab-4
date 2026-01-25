using ProductCart.MAUI.Models;

namespace ProductCart.MAUI.Services.Interfaces;

public interface ICartService
{
    Task<Cart?> GetCartAsync(string userId);
    Task<bool> AddProductToCartAsync(string userId, Guid productId, int quantity);
    Task<bool> RemoveProductFromCartAsync(string userId, Guid productId);
    Task<Cart?> CheckoutCartAsync(string userId);
}
