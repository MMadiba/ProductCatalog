using ProductCatalog.Domain.Entities;

namespace ProductCatalog.Domain.Repositories;

public interface IProductRepository : IRepository<Product>
{
    Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int? categoryId, CancellationToken cancellationToken = default);
    Task<IReadOnlyList<Product>> GetAllForSearchAsync(CancellationToken cancellationToken = default);
}
