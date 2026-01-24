using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using ProductCart.BFF.Application.DTOs;
using ProductCart.BFF.Application.Services.Interfaces;
using System;

namespace ProductCart.BFF.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;
    private readonly ICartAggregationService _cartAggregationService;

    public CartController(
        ICartService cartService,
        ICartAggregationService cartAggregationService)
    {
        _cartService = cartService;
        _cartAggregationService = cartAggregationService;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetCart(string userId)
    {
        var cart = await _cartAggregationService.GetEnrichedCartAsync(userId);

        if (cart == null)
            return NotFound(new { message = $"Cart for user {userId} not found" });

        return Ok(cart);
    }

    [HttpPost("{userId}/items")]
    public async Task<IActionResult> AddItemToCart(
        string userId,
        [FromBody] AddToCartRequest request)
    {
        var success = await _cartService.AddItemToCartAsync(userId, request);

        if (!success)
            return BadRequest(new { message = "Failed to add item to cart" });

        return Ok(new { message = "Item added to cart successfully" });
    }

    [HttpDelete("{userId}/items/{productId}")]
    public async Task<IActionResult> RemoveItemFromCart(string userId, string productId)
    {
        var success = await _cartService.RemoveItemFromCartAsync(userId, productId);

        if (!success)
            return BadRequest(new { message = "Failed to remove item from cart" });

        return Ok(new { message = "Item removed from cart successfully" });
    }

    [HttpPost("{userId}/checkout")]
    public async Task<IActionResult> Checkout(string userId)
    {
        var result = await _cartService.CheckoutCartAsync(userId);

        if (result == null || !result.Success)
            return BadRequest(new { message = result?.Message ?? "Checkout failed" });

        return Ok(result);
    }
}
