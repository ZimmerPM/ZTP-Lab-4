namespace ProductCart.BFF.Infrastructure.Models.Lab3;

public class Lab3CheckoutResponse
{
    public string CartId { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
}
