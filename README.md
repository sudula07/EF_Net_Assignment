# Product API Technical Assessment

## Overview

This repository contains a RESTful Product API built with `.NET 8`, `ASP.NET Core Web API`, `Entity Framework Core`, and `SQL Server`. The solution implements CRUD operations for `Product` entities and their related `Item` records using a layered architecture focused on clarity, maintainability, and testability.

The project includes:

- Product CRUD endpoints
- Entity Framework Core code-first persistence with migrations
- Request validation with FluentValidation
- Centralized exception handling middleware
- Swagger/OpenAPI documentation
- Unit and smoke/integration tests
- Docker and Docker Compose support

Repository:

- `https://github.com/sudula07/EF_Net_Assignment.git`

## Tech Stack

- `.NET 8`
- `ASP.NET Core Web API`
- `Entity Framework Core 8`
- `SQL Server LocalDB / SQL Server`
- `FluentValidation`
- `Swagger / OpenAPI`
- `xUnit`
- `Docker / Docker Compose`

## Solution Structure

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

### Layer Responsibilities

- `src/ProductApi.API`
  Hosts controllers, middleware, startup configuration, and Swagger.
- `src/ProductApi.Application`
  Contains DTOs, service logic, validators, interfaces, and application exceptions.
- `src/ProductApi.Domain`
  Contains core entities and shared domain primitives.
- `src/ProductApi.Infrastructure`
  Contains EF Core configuration, repositories, DbContext, and migrations.
- `tests/ProductApi.Tests`
  Contains unit tests and API smoke/integration coverage.

## Implemented Features

### REST Endpoints

- `GET /api/v1/products?pageNumber=1&pageSize=10`
- `GET /api/v1/products/{id}`
- `POST /api/v1/products`
- `PUT /api/v1/products/{id}`
- `DELETE /api/v1/products/{id}`

### Validation and Error Handling

- Request validation is implemented with `FluentValidation`.
- Errors are handled through custom middleware for consistent API responses.

### Persistence

- The application uses `Entity Framework Core` with SQL Server.
- Product records are stored in the `Products` table.
- Related item rows are stored in the `Items` table.
- Migrations are applied automatically at startup for relational databases.

## Database Details

Default local configuration:

- Server: `(localdb)\MSSQLLocalDB`
- Database: `ProductApiDb`

Connection string source:

- `src/ProductApi.API/appsettings.json`

### Schema Summary

#### Products

- `Id` int PK identity
- `ProductName` nvarchar(200) not null
- `CreatedBy` nvarchar(100) not null
- `CreatedOn` datetime2 not null
- `ModifiedBy` nvarchar(100) null
- `ModifiedOn` datetime2 null

#### Items

- `Id` int PK identity
- `ProductId` int FK to `Products.Id`
- `Quantity` int not null

Relationship:

- One product can contain many items.
- Deleting a product cascades deletion to its items.

## Sample Requests

### Create Product

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

### Update Product

```json
{
  "productName": "Laptop Pro",
  "modifiedBy": "candidate",
  "items": [
    { "quantity": 4 }
  ]
}
```

## Running the Application

### Prerequisites

- `.NET SDK 8`
- SQL Server LocalDB or SQL Server

### Run Locally

```powershell
dotnet build ProductApi.slnx
dotnet run --project src/ProductApi.API/ProductApi.API.csproj
```

Default local Swagger URL:

- `http://localhost:5081/swagger`

### Run with Docker

```powershell
docker compose up --build
```

Expected endpoints:

- API: `http://localhost:8080`
- Swagger: `http://localhost:8080/swagger`
- SQL Server: `localhost,1433`

## Entity Framework Migrations

Restore the local EF tool first:

```powershell
dotnet tool restore
```

Apply migrations:

```powershell
dotnet dotnet-ef database update --project src/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj --startup-project src/ProductApi.API/ProductApi.API.csproj
```

Create a new migration:

```powershell
dotnet dotnet-ef migrations add <MigrationName> --project src/ProductApi.Infrastructure/ProductApi.Infrastructure.csproj --startup-project src/ProductApi.API/ProductApi.API.csproj --output-dir Data/Migrations
```

## Running Tests

```powershell
dotnet test ProductApi.slnx
```

## Notes and Assumptions

- JWT authentication services are wired in the application startup.
- Current CRUD endpoints are left accessible for straightforward local testing.
- Test execution uses EF Core `InMemoryDatabase`, not SQL Server.
- Product update replaces the existing item collection with the submitted payload.
- Docker configuration is intended for technical assessment use.

## Submission Checklist

- Public GitHub repository created
- Source code pushed to GitHub
- Local application run verified
- Screenshot of the running application ready to attach
- Email subject set to `CRN Technical Assessment Complete Successfully`

## Contact Submission

Please share:

- Public repository link
- Screenshot of the API running locally, ideally with Swagger open

