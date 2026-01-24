using ProductCart.MAUI.Models;

namespace ProductCart.MAUI.Services.Interfaces;

public interface IProductService
{
    Task<List<Product>> GetProductsAsync();
    Task<Product?> GetProductByIdAsync(int id);
}
