# Product Catalog Management System

A full-stack Product Catalog for e-commerce: ASP.NET Core Web API backend and Angular SPA frontend.

## Prerequisites

- **.NET 9 SDK** – [Download](https://dotnet.microsoft.com/download)
- **Node.js 18+** and npm
- **Angular CLI 18** (optional): `npm install -g @angular/cli@18`

## Repository layout

```
ProductCatalog/
├── backend/
│   ├── ProductCatalog.Api/          # ASP.NET Core Web API
│   ├── ProductCatalog.Domain/       # Entities, DTOs, interfaces, ProductSearchEngine
│   └── ProductCatalog.Infrastructure/  # Repositories, EF DbContext
├── frontend/
│   └── product-catalog-ui/         # Angular 18 SPA
├── README.md
└── SOLUTION.md
```

## Backend: build and run

```bash
cd ProductCatalog
dotnet build
cd backend/ProductCatalog.Api
dotnet run
```

API runs at **https://localhost:7157** (or the port shown in the console).  
Swagger/OpenAPI (if enabled): https://localhost:7157/openapi/v1.json

## Frontend: build and run

```bash
cd ProductCatalog/frontend/product-catalog-ui
npm install
npm start
```

App runs at **http://localhost:4200**.  
Set the API base URL in `src/environments/environment.development.ts` (`apiUrl`) if your backend uses a different port.

## Tests

**Backend**

```bash
cd ProductCatalog
dotnet test
```

(Add a test project and reference it in the solution if you want automated backend tests.)

**Frontend**

```bash
cd ProductCatalog/frontend/product-catalog-ui
npm test
```

## API endpoints

| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | /api/products | Paginated list; query params: `page`, `pageSize`, `categoryId`, `search` |
| GET | /api/products/{id} | Product by id |
| POST | /api/products | Create product |
| PUT | /api/products/{id} | Update product |
| DELETE | /api/products/{id} | Delete product |
| GET | /api/categories | Flat list of categories |
| GET | /api/categories/tree | Hierarchical category tree |
| POST | /api/categories | Create category |

## Design and trade-offs

See **SOLUTION.md** for architecture, C# and Angular design choices, and trade-offs.
