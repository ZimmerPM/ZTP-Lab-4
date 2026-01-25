namespace ProductCart.MAUI.Models;

public class Cart
{
    public int Id { get; set; }
    public string UserId { get; set; } = string.Empty;
    public List<CartItem> Items { get; set; } = new();

    public decimal TotalValue => Items.Sum(i => i.TotalPrice);
    public string DisplayTotal => $"${TotalValue:F2}";
    public int ItemCount => Items.Sum(i => i.Quantity);
}
