namespace ProductCatalog.Domain.DTOs;

public record CategoryDto(int Id, string Name, string? Description, int? ParentCategoryId);

public record CategoryTreeNodeDto(int Id, string Name, string? Description, int? ParentCategoryId, IReadOnlyList<CategoryTreeNodeDto> Children);

public record CreateCategoryDto(string Name, string? Description, int? ParentCategoryId);
