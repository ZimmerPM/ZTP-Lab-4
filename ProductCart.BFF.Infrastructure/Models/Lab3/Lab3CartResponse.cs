namespace ProductCart.BFF.Infrastructure.Models.Lab3;

public class Lab3CartResponse
{
    public string Id { get; set; } = string.Empty;
    public string UserId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public DateTime LastActivityTime { get; set; }
    public List<Lab3CartItem> Items { get; set; } = new();
}

public class Lab3CartItem
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal TotalPrice { get; set; }
}
