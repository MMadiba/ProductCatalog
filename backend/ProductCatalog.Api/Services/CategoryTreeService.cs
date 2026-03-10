using ProductCatalog.Domain.DTOs;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Api.Services;

public class CategoryTreeService
{
    private readonly ICategoryRepository _repo;

    public CategoryTreeService(ICategoryRepository repo)
    {
        _repo = repo;
    }

    public async Task<IReadOnlyList<CategoryTreeNodeDto>> GetTreeAsync(CancellationToken cancellationToken = default)
    {
        var all = await _repo.GetAllAsync(cancellationToken);
        return BuildTree(all, null);
    }

    private static List<CategoryTreeNodeDto> BuildTree(IReadOnlyList<Category> all, int? parentId)
    {
        return all
            .Where(c => c.ParentCategoryId == parentId)
            .Select(c => new CategoryTreeNodeDto(
                c.Id,
                c.Name,
                c.Description,
                c.ParentCategoryId,
                BuildTree(all, c.Id)))
            .OrderBy(n => n.Name)
            .ToList();
    }
}
