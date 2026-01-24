namespace ProductCart.BFF.Application.DTOs;

public class CartDto
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime LastActivityTime { get; set; }
    public List<CartItemDto> Items { get; set; } = new();
    public decimal GrandTotal => Items.Sum(i => i.TotalPrice);
}
