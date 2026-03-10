namespace ProductCatalog.Domain.Entities;

public class Product : IComparable<Product>
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public int? CategoryId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public int CompareTo(Product? other) => other is null ? 1 : string.Compare(Name, other.Name, StringComparison.OrdinalIgnoreCase);
}
