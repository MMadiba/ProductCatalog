namespace ProductCatalog.Domain.DTOs;

public record ProductDto(
    int Id,
    string Name,
    string? Description,
    string SKU,
    decimal Price,
    int Quantity,
    int? CategoryId,
    DateTime CreatedAt,
    DateTime UpdatedAt
);

public record CreateUpdateProductDto(
    string Name,
    string? Description,
    string SKU,
    decimal Price,
    int Quantity,
    int? CategoryId
);