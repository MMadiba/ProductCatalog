using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;

namespace ProductCatalog.Infrastructure.Repositories;

public class InMemoryProductRepository : IProductRepository
{
    private readonly List<Product> _list = new();
    private readonly Dictionary<int, Product> _byId = new();
    private int _nextId = 1;
    private readonly object _sync = new();

    public Task<Product?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(_byId.TryGetValue(id, out var p) ? p : null);
    }

    public Task<IReadOnlyList<Product>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Product>>(_list.ToList());
    }

    public Task<IReadOnlyList<Product>> GetByCategoryIdAsync(int? categoryId, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            var list = _list.Where(p => p.CategoryId == categoryId).ToList();
            return Task.FromResult<IReadOnlyList<Product>>(list);
        }
    }

    public Task<IReadOnlyList<Product>> GetAllForSearchAsync(CancellationToken cancellationToken = default)
    {
        return Task.FromResult<IReadOnlyList<Product>>(_list.ToList());
    }

    public Task<Product> AddAsync(Product entity, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            entity.Id = _nextId++;
            entity.CreatedAt = DateTime.UtcNow;
            entity.UpdatedAt = entity.CreatedAt;
            _list.Add(entity);
            _byId[entity.Id] = entity;
            return Task.FromResult(entity);
        }
    }

    public Task UpdateAsync(Product entity, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            if (!_byId.TryGetValue(entity.Id, out var existing))
                throw new InvalidOperationException($"Product {entity.Id} not found.");
            var oldCat = existing.CategoryId;
            existing.Name = entity.Name;
            existing.Description = entity.Description;
            existing.SKU = entity.SKU;
            existing.Price = entity.Price;
            existing.Quantity = entity.Quantity;
            existing.CategoryId = entity.CategoryId;
            existing.UpdatedAt = DateTime.UtcNow;
            return Task.CompletedTask;
        }
    }

    public Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        lock (_sync)
        {
            if (!_byId.TryGetValue(id, out var p))
                return Task.CompletedTask;
            _list.Remove(p);
            _byId.Remove(id);
            return Task.CompletedTask;
        }
    }
}
