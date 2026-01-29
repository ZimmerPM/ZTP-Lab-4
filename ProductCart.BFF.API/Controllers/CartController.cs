using Microsoft.AspNetCore.Mvc;
using ProductCart.BFF.Infrastructure.HttpClients;
using ProductCart.BFF.Infrastructure.Models.Lab3;

namespace ProductCart.BFF.API.Controllers;

[ApiController]
[Route("")]
public class CartController : ControllerBase
{
    private readonly CartApiClient _cartApiClient;

    public CartController(CartApiClient cartApiClient)
    {
        _cartApiClient = cartApiClient;
    }

    [HttpGet("carts/{cartId}")]
    public async Task<IActionResult> GetCart(Guid cartId)
    {
        var cart = await _cartApiClient.GetCartAsync(cartId.ToString());

        if (cart == null)
            return NotFound(new { message = $"Cart {cartId} not found" });

        return Ok(cart);
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
}
