namespace ProductCart.MAUI.Models;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Category { get; set; }

    public string DisplayPrice => $"${Price:F2}";

    public ProductCategory CategoryEnum => (ProductCategory)Category;

    public string CategoryName => CategoryEnum switch
    {
        ProductCategory.Electronics => "Electronics",
        ProductCategory.Books => "Books",
        ProductCategory.Clothing => "Clothing",
        _ => "Unknown"
    };
}
