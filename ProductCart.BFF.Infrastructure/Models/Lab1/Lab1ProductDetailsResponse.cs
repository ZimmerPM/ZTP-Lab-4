namespace ProductCart.BFF.Infrastructure.Models.Lab1;

public class Lab1ProductDetailsResponse
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Category { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
}
