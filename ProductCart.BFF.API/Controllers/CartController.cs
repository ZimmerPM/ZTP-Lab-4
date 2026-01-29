using Microsoft.AspNetCore.Mvc;
using ProductCart.BFF.Infrastructure.HttpClients;
using ProductCart.BFF.Infrastructure.Models.Lab3;
using ProductCart.BFF.Application.DTOs;

namespace ProductCart.BFF.API.Controllers;

[ApiController]
[Route("")]
public class CartController : ControllerBase
{
    private readonly CartApiClient _cartApiClient;
    private readonly ProductApiClient _productApiClient;

    public CartController(
        CartApiClient cartApiClient,
        ProductApiClient productApiClient)
    {
        _cartApiClient = cartApiClient;
        _productApiClient = productApiClient;
    }

    [HttpGet("carts/{cartId}")]
    public async Task<IActionResult> GetCart(Guid cartId)
    {
        var cart = await _cartApiClient.GetCartAsync(cartId.ToString());

        if (cart == null)
            return NotFound(new { message = $"Cart {cartId} not found" });

        var enrichedItems = new List<CartItemDto>();

        foreach (var item in cart.Items)
        {
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

            enrichedItems.Add(new CartItemDto
            {
                ProductId = item.ProductId,
                ProductName = item.ProductName,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                TotalPrice = item.TotalPrice
            });
        }

        var response = new
        {
            cartId = cart.Id,
            userId = cart.UserId,
            items = enrichedItems,
            totalAmount = enrichedItems.Sum(i => i.TotalPrice)
        };

        return Ok(response);
    }

    [HttpPost("carts/{cartId}/items")]
    public async Task<IActionResult> AddItemToCart(
        Guid cartId,
        [FromQuery] Guid userId,
        [FromBody] Lab3AddItemRequest request)
    {
        var success = await _cartApiClient.AddItemToCartAsync(
            cartId.ToString(),
            userId.ToString(),
            request);

        if (!success)
            return BadRequest(new { message = "Failed to add item to cart" });

        return Ok(new
        {
            cartId = cartId,
            success = true,
            message = "Product added to cart"
        });
    }

    [HttpDelete("carts/{cartId}/items/{productId}")]
    public async Task<IActionResult> RemoveItemFromCart(
        Guid cartId,
        Guid productId,
        [FromQuery] Guid userId)
    {
        var success = await _cartApiClient.RemoveItemFromCartAsync(
            cartId.ToString(),
            productId.ToString());

        if (!success)
            return BadRequest(new { message = "Failed to remove item from cart" });

        return Ok(new { message = "Item removed from cart successfully" });
    }

    [HttpPost("carts/{cartId}/checkout")]
    public async Task<IActionResult> Checkout(
        Guid cartId,
        [FromQuery] Guid userId)
    {
        var result = await _cartApiClient.CheckoutCartAsync(
            cartId.ToString(),
            userId.ToString());

        if (result == null || !result.Success)
            return BadRequest(new { message = result?.Message ?? "Checkout failed" });

        return Ok(result);
    }

    private static int? ExtractProductIdFromGuid(string guidString)
    {
        if (Guid.TryParse(guidString, out var guid))
        {
            var bytes = guid.ToByteArray();
            var productId = BitConverter.ToInt32(bytes, 0);
            return productId;
        }
        return null;
    }
}
