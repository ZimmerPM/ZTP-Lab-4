namespace ProductCart.MAUI.Models;

public class CartItem
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }

    public decimal TotalPrice => Price * Quantity;
    public string DisplayPrice => $"${Price:F2}";
    public string DisplayTotal => $"${TotalPrice:F2}";
}
