using Microsoft.AspNetCore.Mvc;
using ProductCart.BFF.Application.Services.Interfaces;
using ProductCart.BFF.Infrastructure.HttpClients;

namespace ProductCart.BFF.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductService _productService;
    private readonly CartApiClient _cartApiClient; 

    public ProductsController(
        IProductService productService,
        CartApiClient cartApiClient)  
    {
        _productService = productService;
        _cartApiClient = cartApiClient;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllProducts()
    {
        var products = await _productService.GetAllProductsAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetProductById(int id, [FromQuery] Guid? cartId = null)
    {
        var product = await _productService.GetProductByIdAsync(id);

        if (product == null)
            return NotFound(new { message = $"Product with ID {id} not found" });

        if (!cartId.HasValue)
            return Ok(product);

        int quantityInCart = 0;
        var cart = await _cartApiClient.GetCartAsync(cartId.Value.ToString());

        if (cart != null)
        {
            var productIdGuid = $"{id:D8}-0000-0000-0000-000000000000";
            var cartItem = cart.Items.FirstOrDefault(i =>
                i.ProductId.Equals(productIdGuid, StringComparison.OrdinalIgnoreCase));

            if (cartItem != null)
                quantityInCart = cartItem.Quantity;
        }

        var enrichedProduct = new
        {
            id = product.Id,
            name = product.Name,
            price = product.Price,
            category = product.Category,
            quantity = product.Quantity,
                                         
            isInCart = quantityInCart > 0,
            quantityInCart = quantityInCart,
            cartSubtotal = product.Price * quantityInCart
        };

        return Ok(enrichedProduct);
    }
}
