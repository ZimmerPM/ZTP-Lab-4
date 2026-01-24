namespace ProductCart.BFF.Application.DTOs;

public class CheckoutResponse
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string? OrderId { get; set; }
}
