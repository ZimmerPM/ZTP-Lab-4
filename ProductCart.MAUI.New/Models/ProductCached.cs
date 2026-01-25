using SQLite;

namespace ProductCart.MAUI.Models;

[Table("products_cache")]
public class ProductCached
{
    [PrimaryKey]
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal Price { get; set; }

    public int Category { get; set; }

    public int Quantity { get; set; }

    public DateTime LastUpdated { get; set; }

    public static ProductCached FromProduct(Product product)
    {
        return new ProductCached
        {
            Id = product.Id,
            Name = product.Name,
            Price = product.Price,
            Category = product.Category,
            Quantity = product.Quantity,
            LastUpdated = DateTime.UtcNow
        };
    }

    public Product ToProduct()
    {
        return new Product
        {
            Id = this.Id,
            Name = this.Name,
            Price = this.Price,
            Category = this.Category,
            Quantity = this.Quantity
        };
    }
}
