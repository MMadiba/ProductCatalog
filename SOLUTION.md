# Solution: Product Catalog Management System

## High-level design

- **Backend**: ASP.NET Core 9 Web API with a small domain layer (entities, DTOs, repository interfaces, `ProductSearchEngine`), and an infrastructure layer (repositories, EF Core in-memory for categories, pure in-memory for products).
- **Frontend**: Angular 18 standalone SPA with reactive forms, services calling the API, and simple product/category UI.

## Backend design and requirements mapping

### Repository pattern

- **`IRepository<T>`** – generic interface with `GetById`, `GetAll`, `Add`, `Update`, `Delete`.
- **`RepositoryBase<T>`** – abstract base in Infrastructure implementing the interface (no framework; just contract).
- **In-memory product repository**: `InMemoryProductRepository` uses **`List<Product>`** and **`Dictionary<int, Product>`** only (no EF), satisfying “at least one repository must use pure in-memory collections.”
- **Categories**: `EfCategoryRepository` with **EF Core InMemory** and `CatalogDbContext`, so “most” data access uses EF as required.

### ProductSearchEngine (core C# / BCL only)

- **Generic** `ProductSearchEngine<T>` so it can be reused for other entity types.
- **Fuzzy matching**: subsequence-style match (e.g. `"lptop"` matches `"laptop"`) with normalized text (letters/digits only).
- **Weighted scoring**: configurable weights per field (e.g. Name=10, Description=3, SKU=5); scores summed and results ordered by score.
- Implemented with **only .NET BCL** (no external NuGet).

### LINQ extension methods

- **`ProductQueryExtensions`**: `FilterByCategory`, `FilterByNameContains`, `InStock` for both `IQueryable<Product>` and `IEnumerable<Product>` to support in-memory product filtering.

### DTOs and nullable reference types

- DTOs are **record types** (e.g. `ProductDto`, `CreateProductDto`, `PagedResult<T>`).
- **Nullable reference types** are enabled and used across the solution.

### Pattern matching for validation

- In `ProductsController`, **request validation** uses switch pattern matching on the DTO (e.g. `{ Name: null or "" }`, `{ Price: < 0 }`) to return a validation error object or `null` when valid.

### Custom middleware

- **`RequestLoggingMiddleware`** implemented from scratch: takes `RequestDelegate` and `ILogger`, invokes the next delegate, and logs method, path, status code, and elapsed time. Registered via a custom extension that composes the pipeline without using built-in logging middleware.

### Caching

- **`SearchCacheService`** uses a **`ConcurrentDictionary<string, (object Result, DateTime Expires)>`** to cache search results by key (search term, categoryId, page, pageSize) with a 5-minute TTL. Products list action checks cache before calling the repository/search.

### Category tree

- **`CategoryTreeService`** builds a hierarchy from the flat category list using `ParentCategoryId`. API exposes **GET /api/categories** (flat) and **GET /api/categories/tree** (nested `CategoryTreeNodeDto` with `Children`).

### IComparable for products

- **`Product`** implements **`IComparable<Product>`** (compare by `Name` case-insensitive). Used when ordering product lists (e.g. after search).

### DI and ProductSearchEngine

- **ProductSearchEngine** is registered in **DI** with a preconfigured dictionary of field selectors and weights for `Product`. It is injected into `ProductsController`; the controller calls `SetSource(allProducts)` before search and uses the engine for weighted, fuzzy search when a search term is provided.

## Frontend design

- **Angular 18**, **standalone components**, **reactive forms** with validation.
- **TypeScript interfaces** for Product, Category, PagedResult, etc., aligned with API response shapes (camelCase from ASP.NET Core).
- **RxJS** for all HTTP calls; **error handling** via `catchError` and simple error/success signals in the UI.
- **Product list**: search input, category dropdown filter, pagination, table with Edit/Delete (delete with browser `confirm`).
- **Product form**: single component for add/edit; route params determine mode; reactive form with required and min validators.
- **Categories**: tree display and a simple “add category” form with parent dropdown.
- **One unit test**: `ProductService` spec that checks `getPage` issues a GET with correct query params.

## Trade-offs

1. **Products in-memory**: Products are not persisted across restarts; categories are in EF InMemory, so also not persisted. Acceptable for a take-home and demo; production would use a real DB and persistent product store.
2. **Search cache**: In-memory cache is per process and not distributed; sufficient for a single-instance API.
3. **Fuzzy algorithm**: Simple subsequence + exact substring scoring; no Levenshtein or full-text engine to stay within “BCL only” and keep the demo focused.
4. **Auth**: Skipped as per instructions.
5. **UI**: Minimal styling and no design system; focuses on structure, forms, and behavior.

## How to run (summary)

1. From repo root: `dotnet build` then run `ProductCatalog.Api`.
2. In `frontend/product-catalog-ui`: `npm install` and `npm start`.

3. Open the Angular app and ensure `apiUrl` in the environment points to the running API (e.g. https://localhost:7157/api).
