# Product API Technical Assessment

## Project overview

This repository contains a layered `.NET 8` RESTful Product API built with `ASP.NET Core Web API`, `Entity Framework Core`, and `SQL Server`. It implements CRUD operations for `Product` entities and their related `Item` records, with validation, consistent error handling, Swagger, Docker assets, and automated tests.

## Tech stack

- .NET 8
- ASP.NET Core Web API
- Entity Framework Core 8
- SQL Server / LocalDB
- FluentValidation
- Swagger / OpenAPI
- xUnit
- Docker / Docker Compose

## Architecture

The solution follows a simple clean layered structure:

- `src/ProductApi.API`
  Hosts controllers, middleware, Swagger, authentication wiring, and startup configuration.
- `src/ProductApi.Application`
  Contains DTOs, service logic, validators, interfaces, and application exceptions.
- `src/ProductApi.Domain`
  Contains core entities and domain primitives.
- `src/ProductApi.Infrastructure`
  Contains EF Core, repository implementation, DbContext, and migrations.
- `tests/ProductApi.Tests`
  Contains unit tests and an API smoke/integration test.

## Folder structure

```text
.
|-- src
|   |-- ProductApi.API
|   |-- ProductApi.Application
|   |-- ProductApi.Domain
|   `-- ProductApi.Infrastructure
|-- tests
|   `-- ProductApi.Tests
|-- Dockerfile
|-- docker-compose.yml
|-- ProductApi.slnx
`-- README.md
```

## Database schema

### Products

- `Id` int PK identity
- `ProductName` nvarchar(200) not null
- `CreatedBy` nvarchar(100) not null
- `CreatedOn` datetime2 not null
- `ModifiedBy` nvarchar(100) null
- `ModifiedOn` datetime2 null

### Items

- `Id` int PK identity
- `ProductId` int FK to `Products.Id`
- `Quantity` int not null

Relationship:

- One `Product` has many `Items`
- Deleting a `Product` cascades to its `Items`

## API endpoints

- `GET /api/v1/products?pageNumber=1&pageSize=10`
- `GET /api/v1/products/{id}`
- `POST /api/v1/products`
- `PUT /api/v1/products/{id}`
- `DELETE /api/v1/products/{id}`

## Example payloads

### Create product

```json
{
  "productName": "Laptop",
  "createdBy": "candidate",
  "items": [
    { "quantity": 2 },
    { "quantity": 5 }
  ]
}
```

### Update product

```json
{
  "productName": "Laptop Pro",
  "modifiedBy": "candidate",
  "items": [
    { "quantity": 4 }
  ]
}
```

## How to run locally

Prerequisites:

- `.NET SDK` with `net8.0` support
- SQL Server LocalDB or SQL Server

Steps:

```powershell
dotnet build ProductApi.slnx
dotnet run --project src/ProductApi.API/ProductApi.API.csproj
```

Default local Swagger URL:

- `http://localhost:5081/swagger`

The API applies pending EF migrations automatically on startup for relational databases.

## How to run with Docker

```powershell
docker compose up --build
```

Expected URLs:

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- SQL Server: `localhost,1433`

## How to apply migrations

Local EF CLI is configured through `dotnet-tools.json`.

```powershell
dotnet tool restore
dotnet dotnet-ef database update --project src/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj --startup-project src/ProductApi.API/ProductApi.API.csproj
```

To add a new migration:

```powershell
dotnet dotnet-ef migrations add <MigrationName> --project src/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj --startup-project src/ProductApi.API/ProductApi.API.csproj --output-dir Data/Migrations
```

## How to run tests

```powershell
dotnet test ProductApi.slnx
```

## Smoke test examples

```powershell
curl.exe http://localhost:5081/api/v1/products
```

```powershell
curl.exe -X POST http://localhost:5081/api/v1/products `
  -H "Content-Type: application/json" `
  -d "{\"productName\":\"Phone\",\"createdBy\":\"smoke-test\",\"items\":[{\"quantity\":3}]}"
```

```powershell
curl.exe http://localhost:5081/swagger
```

## Known assumptions

- JWT authentication wiring is included but CRUD endpoints are left open for easy smoke testing.
- Update requests replace the product's item collection with the submitted list.
- Local development uses LocalDB by default.
- Docker uses placeholder local-development credentials only.

## Submission note

This solution is intentionally assessment-ready: straightforward layered design, async CRUD flow, EF Core migrations, tests, Docker assets, and documentation with minimal ceremony.
