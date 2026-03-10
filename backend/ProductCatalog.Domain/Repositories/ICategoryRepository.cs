using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Repositories;

public interface ICategoryRepository : IRepository<Category>
{
    Task<IReadOnlyList<Category>> GetChildrenAsync(int? parentId, CancellationToken cancellationToken = default);
}
