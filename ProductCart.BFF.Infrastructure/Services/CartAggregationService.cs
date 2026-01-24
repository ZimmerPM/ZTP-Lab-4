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
            // Konwersja GUID → int (wyciągnij ostatnie 12 cyfr)
            var productId = ExtractProductIdFromGuid(item.ProductId);

            if (productId.HasValue)
            {
                var productDetails = await _productApiClient.GetProductByIdAsync(productId.Value);

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
                    continue;
                }
            }

            // Fallback: użyj danych z koszyka
            enrichedItems.Add(new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            });
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

    private static int? ExtractProductIdFromGuid(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            var guidStr = guid.ToString();
            var lastPart = guidStr.Split('-').Last();

            if (int.TryParse(lastPart, out int productId))
                return productId;
        }

        return null;
    }
}
