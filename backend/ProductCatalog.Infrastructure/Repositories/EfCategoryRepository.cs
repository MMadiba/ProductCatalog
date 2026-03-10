using Microsoft.EntityFrameworkCore;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;
using ProductCatalog.Infrastructure.Data;

namespace ProductCatalog.Infrastructure.Repositories;

public class EfCategoryRepository : ICategoryRepository
{
    private readonly CatalogDbContext _db;

    public EfCategoryRepository(CatalogDbContext db)
    {
        _db = db;
    }

    public async Task<Category?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _db.Categories.OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<Category>> GetChildrenAsync(int? parentId, CancellationToken cancellationToken = default)
    {
        return await _db.Categories.Where(c => c.ParentCategoryId == parentId).OrderBy(c => c.Name).ToListAsync(cancellationToken);
    }

    public async Task<Category> AddAsync(Category entity, CancellationToken cancellationToken = default)
    {
        if (entity.Id == 0)
            entity.Id = await _db.Categories.AnyAsync(cancellationToken)
                ? await _db.Categories.MaxAsync(c => c.Id, cancellationToken) + 1
                : 1;
        _db.Categories.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(Category entity, CancellationToken cancellationToken = default)
    {
        _db.Categories.Update(entity);
        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken = default)
    {
        var c = await _db.Categories.FindAsync(new object[] { id }, cancellationToken);
        if (c is not null)
        {
            _db.Categories.Remove(c);
            await _db.SaveChangesAsync(cancellationToken);
        }
    }
}
