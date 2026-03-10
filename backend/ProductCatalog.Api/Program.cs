using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ProductCatalog.Domain.Entities;
using ProductCatalog.Domain.Repositories;
using ProductCatalog.Domain.Search;
using ProductCatalog.Infrastructure.Data;
using ProductCatalog.Infrastructure.Repositories;
using ProductCatalog.Api.Middleware;
using ProductCatalog.Api.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddOpenApi();
builder.Services.AddCors();

builder.Services.AddDbContext<CatalogDbContext>(o => o.UseInMemoryDatabase("Catalog"));
builder.Services.AddSingleton<IProductRepository, InMemoryProductRepository>();
builder.Services.AddScoped<ICategoryRepository, EfCategoryRepository>();
builder.Services.AddSingleton<SearchCacheService>();
builder.Services.AddScoped<CategoryTreeService>();

// ProductSearchEngine: weighted fields (Name=10, Description=3, SKU=5) - DI as singleton with Product type
var productWeights = new Dictionary<Func<Product, string>, int>
{
    [p => p.Name] = 10,
    [p => p.Description ?? ""] = 3,
    [p => p.SKU] = 5
};
builder.Services.AddSingleton(new ProductSearchEngine<Product>(productWeights));

var app = builder.Build();

app.UseRequestLogging();
app.UseCors(policy => policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
app.UseHttpsRedirection();
app.MapControllers();

if (app.Environment.IsDevelopment())
    app.MapOpenApi();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();
    if (!await db.Categories.AnyAsync())
    {
        db.Categories.AddRange(
            new Category { Id = 1, Name = "Electronics", Description = "Electronic devices", ParentCategoryId = null },
            new Category { Id = 2, Name = "Computers", Description = "PCs and laptops", ParentCategoryId = 1 },
            new Category { Id = 3, Name = "Books", Description = "Books and media", ParentCategoryId = null });
        await db.SaveChangesAsync();
    }
}

app.Run();
