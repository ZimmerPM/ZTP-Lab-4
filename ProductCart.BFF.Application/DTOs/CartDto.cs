namespace ProductCart.BFF.Application.DTOs;

public class CartDto
{
    public string CartId { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public List<CartItemDto> Items { get; set; } = new();
    public decimal Total => Items.Sum(i => i.Subtotal);
}
