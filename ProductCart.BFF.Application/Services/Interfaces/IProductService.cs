using ProductCart.BFF.Application.DTOs;

namespace ProductCart.BFF.Application.Services.Interfaces;

public interface IProductService
{
    Task<List<ProductListItemDto>> GetAllProductsAsync();
    Task<ProductDto?> GetProductByIdAsync(int productId);
}
