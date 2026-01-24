using ProductCart.BFF.Application.DTOs;
using ProductCart.BFF.Application.Services.Interfaces;
using ProductCart.BFF.Infrastructure.HttpClients;


namespace ProductCart.BFF.Infrastructure.Services;

public class CartAggregationService : ICartAggregationService
{
    private readonly CartApiClient _cartApiClient;
    private readonly ProductApiClient _productApiClient;

    public CartAggregationService(
        CartApiClient cartApiClient,
        ProductApiClient productApiClient)
    {
        _cartApiClient = cartApiClient;
        _productApiClient = productApiClient;
    }

    public async Task<CartDto?> GetEnrichedCartAsync(string userId)
    {
        var cart = await _cartApiClient.GetCartAsync(userId);

        if (cart == null)
            return null;

        var enrichedItems = new List<CartItemDto>();

        foreach (var item in cart.Items)
        {
            if (int.TryParse(item.ProductId, out int productId))
            {
                var productDetails = await _productApiClient.GetProductByIdAsync(productId);

                if (productDetails != null)
                {
                    enrichedItems.Add(new CartItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = productDetails.Name,
                        Quantity = item.Quantity,
                        UnitPrice = productDetails.Price,
                        TotalPrice = productDetails.Price * item.Quantity
                    });
                }
                else
                {
                    enrichedItems.Add(new CartItemDto
                    {
                        ProductId = item.ProductId,
                        ProductName = item.ProductName,
                        Quantity = item.Quantity,
                        UnitPrice = item.UnitPrice,
                        TotalPrice = item.TotalPrice
                    });
                }
            }
        }

        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Status = cart.Status,
            LastActivityTime = cart.LastActivityTime,
            Items = enrichedItems
        };
    }
}
