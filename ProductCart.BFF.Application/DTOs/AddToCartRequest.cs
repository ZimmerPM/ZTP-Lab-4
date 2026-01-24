namespace ProductCart.BFF.Application.DTOs;

public class AddToCartRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; } = 1;
}
