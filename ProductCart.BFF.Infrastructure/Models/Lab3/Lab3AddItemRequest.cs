namespace ProductCart.BFF.Infrastructure.Models.Lab3;

public class Lab3AddItemRequest
{
    public string ProductId { get; set; } = string.Empty;
    public int Quantity { get; set; }
}
