using ProductCart.BFF.Application.DTOs;
using ProductCart.BFF.Application.Services.Interfaces;
using ProductCart.BFF.Infrastructure.HttpClients;

namespace ProductCart.BFF.Infrastructure.Services;

public class ProductService : IProductService
{
    private readonly ProductApiClient _productApiClient;

    public ProductService(ProductApiClient productApiClient)
    {
        _productApiClient = productApiClient;
    }

    public async Task<List<ProductListItemDto>> GetAllProductsAsync()
    {
        var products = await _productApiClient.GetProductsAsync();

        return products.Select(p => new ProductListItemDto
        {
            Id = p.Id,
            Name = p.Name,
            Price = p.Price,
            Category = p.Category
        }).ToList();
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId)
    {
        var product = await _productApiClient.GetProductByIdAsync(productId);

        if (product == null)
            return null;

        return new ProductDto
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            Quantity = product.Quantity
        };
    }
}
