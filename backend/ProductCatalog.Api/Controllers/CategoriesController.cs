using Microsoft.AspNetCore.Mvc;
using ProductCatalog.Domain.DTOs;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;
using ProductCatalog.Api.Services;

namespace ProductCatalog.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly ICategoryRepository _repo;
    private readonly CategoryTreeService _treeService;

    public CategoriesController(ICategoryRepository repo, CategoryTreeService treeService)
    {
        _repo = repo;
        _treeService = treeService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetCategories(CancellationToken cancellationToken = default)
    {
        var list = await _repo.GetAllAsync(cancellationToken);
        return Ok(list.Select(ToDto).ToList());
    }

    [HttpGet("tree")]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryTreeNodeDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryTreeNodeDto>>> GetTree(CancellationToken cancellationToken = default)
    {
        var tree = await _treeService.GetTreeAsync(cancellationToken);
        return Ok(tree);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryDto>> GetById(int id, CancellationToken cancellationToken = default)
    {
        var c = await _repo.GetByIdAsync(id, cancellationToken);
        if (c is null) return NotFound();
        return Ok(ToDto(c));
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryDto>> Create(
        [FromBody] CreateCategoryDto dto,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(dto.Name))
            return BadRequest(new { Error = "Name is required" });

        var entity = new Category
        {
            Name = dto.Name.Trim(),
            Description = string.IsNullOrWhiteSpace(dto.Description) ? null : dto.Description.Trim(),
            ParentCategoryId = dto.ParentCategoryId
        };
        entity = await _repo.AddAsync(entity, cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = entity.Id }, ToDto(entity));
    }

    private static CategoryDto ToDto(Category c) => new(c.Id, c.Name, c.Description, c.ParentCategoryId);
}
