using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Infrastructure.Repositories;

public abstract class RepositoryBase<T> : IRepository<T> where T : class
{
    public abstract Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
    public abstract Task<IReadOnlyList<T>> GetAllAsync(CancellationToken cancellationToken = default);
    public abstract Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);
    public abstract Task UpdateAsync(T entity, CancellationToken cancellationToken = default);
    public abstract Task DeleteAsync(int id, CancellationToken cancellationToken = default);
}

