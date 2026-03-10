using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.DTOs;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Linq;
using ProductCatalog.Domain.Repositories;
using ProductCatalog.Domain.Search;
using ProductCatalog.Api.Services;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IProductRepository _repo;
    private readonly ProductSearchEngine<Product> _searchEngine;
    private readonly SearchCacheService _cache;

    public ProductsController(
        IProductRepository repo,
        ProductSearchEngine<Product> searchEngine,
        SearchCacheService cache)
    {
        _repo = repo;
        _searchEngine = searchEngine;
        _cache = cache;
    }

    /// <summary>
    /// GET /api/products - pagination, filter by category, search by name. Uses cache for search results.
    /// </summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductDto>>> GetProducts(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] int? categoryId = null,
        [FromQuery] string? search = null,
        CancellationToken cancellationToken = default)
    {
        page = Math.Max(1, page);
        pageSize = Math.Clamp(pageSize, 1, 100);
        pageSize = Math.Clamp(pageSize, 1, 100);

        var cacheKey = _cache.BuildKey(search, categoryId, page, pageSize);
        var cached = _cache.Get<PagedResult<ProductDto>>(cacheKey);
        if (cached is not null && cached.TotalCount > 0)
            return Ok(cached);

        var all = await _repo.GetAllForSearchAsync(cancellationToken);
        var filtered = all
            .FilterByCategory(categoryId)
            //.FilterByNameContains(search)
            .ToList();

        IEnumerable<Product> forPage;
        if (!string.IsNullOrWhiteSpace(search))
        {
            _searchEngine.SetSource(filtered);
            var searchResults = _searchEngine.Search(search, maxResults: 1000);
            forPage = searchResults
                .OrderBy(p => p)
                .Skip((page - 1) * pageSize)
                .Take(pageSize);
        }
        else
        {
            var sorted = filtered.OrderBy(p => p).ToList();
            forPage = sorted.Skip((page - 1) * pageSize).Take(pageSize);
        }

        var list = forPage.Select(ToDto).ToList();
        var result = new PagedResult<ProductDto>(list, filtered.Count, page, pageSize);
        _cache.Set(cacheKey, result);
        return Ok(result);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var p = await _repo.GetByIdAsync(id, cancellationToken);
        if (p is null) return NotFound();
        return Ok(ToDto(p));
    }

    [HttpPost]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ProductDto>> Create(
        [FromBody] CreateUpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateCreateUpdate(dto);
        if (validation is { } err)
            return BadRequest(err);

        var entity = new Product
        {
            Name = dto.Name,
            Description = dto.Description,
            SKU = dto.SKU,
            Price = dto.Price,
            Quantity = dto.Quantity,
            CategoryId = dto.CategoryId
        };
        entity = await _repo.AddAsync(entity, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDto>> Update(
        int id,
        [FromBody] CreateUpdateProductDto dto,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateCreateUpdate(dto);
        if (validation is { } err)
            return BadRequest(err);

        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing is null) return NotFound();

        existing.Name = dto.Name;
        existing.Description = dto.Description;
        existing.SKU = dto.SKU;
        existing.Price = dto.Price;
        existing.Quantity = dto.Quantity;
        existing.CategoryId = dto.CategoryId;
        await _repo.UpdateAsync(existing, cancellationToken);
        return Ok(ToDto(existing));
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken = default)
    {
        var existing = await _repo.GetByIdAsync(id, cancellationToken);
        if (existing is null) return NotFound();
        await _repo.DeleteAsync(id, cancellationToken);
        return NoContent();
    }

 
    private static object? ValidateCreateUpdate(CreateUpdateProductDto dto)
    {
        return dto switch
        {
            { Name: null or "" } => new { Error = "Name is required" },
            { SKU: null or "" } => new { Error = "SKU is required" },
            { Price: < 0 } => new { Error = "Price must be non-negative" },
            { Quantity: < 0 } => new { Error = "Quantity must be non-negative" },
            _ => null
        };
    }

    private static ProductDto ToDto(Product p) => new(
        p.Id, p.Name, p.Description, p.SKU, p.Price, p.Quantity, p.CategoryId, p.CreatedAt, p.UpdatedAt);
}

public record PagedResult<T>(IReadOnlyList<T> Items, int TotalCount, int Page, int PageSize);
public class ProductQueryBinding
{
    public int Page { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public int? CategoryId { get; set; }
    public string? Search { get; set; }
}
