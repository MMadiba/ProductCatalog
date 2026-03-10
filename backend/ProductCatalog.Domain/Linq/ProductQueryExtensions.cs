using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Linq;

public static class ProductQueryExtensions
{
    public static IQueryable<Product> FilterByCategory(this IQueryable<Product> source, int? categoryId)
    {
        if (!categoryId.HasValue) return source;
        return source.Where(p => p.CategoryId == categoryId.Value);
    }

    public static IQueryable<Product> FilterByNameContains(this IQueryable<Product> source, string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return source;
        var n = name.Trim().ToLower();
        return source.Where(p => p.Name.ToLower().Contains(n));
    }

    public static IQueryable<Product> InStock(this IQueryable<Product> source)
    {
        return source.Where(p => p.Quantity > 0);
    }

    public static IEnumerable<Product> FilterByCategory(this IEnumerable<Product> source, int? categoryId)
    {
        if (!categoryId.HasValue) return source;
        return source.Where(p => p.CategoryId == categoryId.Value);
    }

    public static IEnumerable<Product> FilterByNameContains(this IEnumerable<Product> source, string? name)
    {
        if (string.IsNullOrWhiteSpace(name)) return source;
        var n = name.Trim().ToLower();
        return source.Where(p => p.Name.ToLower().Contains(n));
    }
}
