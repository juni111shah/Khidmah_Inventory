# Architecture Documentation

## Overview

This document describes the architecture of the Khidmah Inventory Management System, a multi-tenant inventory management solution built with Clean Architecture principles.

## Architecture Layers

### 1. Domain Layer (`Khidmah_Inventory.Domain`)

**Purpose**: Contains the core business logic and entities.

**Components**:
- **Entities**: Domain models representing business concepts (User, Product, Order, etc.)
- **Value Objects**: Immutable objects that represent domain concepts
- **Domain Events**: Events that represent something important that happened in the domain
- **Interfaces**: Domain service interfaces

**Key Principles**:
- No dependencies on other layers
- Pure business logic
- Entities are rich domain models with behavior
- All entities inherit from `BaseEntity` which provides:
  - `Id` (Guid)
  - `CompanyId` (for multi-tenancy)
  - Audit fields (CreatedAt, UpdatedAt, CreatedBy, UpdatedBy)
  - Soft delete support

### 2. Application Layer (`Khidmah_Inventory.Application`)

**Purpose**: Contains application use cases and business logic orchestration.

**Components**:
- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, List, Search)
- **Handlers**: CQRS handlers that process commands/queries
- **DTOs**: Data Transfer Objects for API communication
- **Validators**: FluentValidation validators
- **Mappings**: AutoMapper profiles
- **Interfaces**: Application service interfaces

**Pattern**: CQRS (Command Query Responsibility Segregation)
- Commands modify state
- Queries read state
- MediatR is used for dispatching commands/queries

**Example Structure**:
```
Features/
  Auth/
    Commands/
      Login/
        LoginCommand.cs
        LoginCommandHandler.cs
        LoginCommandValidator.cs
      Register/
        ...
    Queries/
      GetCurrentUser/
        ...
  Products/
    Commands/
      CreateProduct/
      UpdateProduct/
      DeleteProduct/
    Queries/
      GetProduct/
      GetProducts/
```

### 3. Infrastructure Layer (`Khidmah_Inventory.Infrastructure`)

**Purpose**: Contains implementations of external concerns.

**Components**:
- **Data**: EF Core DbContext, configurations, interceptors
- **Services**: External service implementations (Identity, Email, File Storage)
- **Repositories**: Data access implementations (if needed)

**Key Features**:
- **EF Core DbContext**: `ApplicationDbContext` implements `IApplicationDbContext`
- **Entity Configurations**: Fluent API configurations for entities
- **Interceptors**:
  - `AuditableEntitySaveChangesInterceptor`: Automatically sets audit fields
  - `MultiTenantInterceptor`: Automatically sets CompanyId for new entities
- **Services**:
  - `IdentityService`: Password hashing, JWT token generation
  - `CurrentUserService`: Gets current user from HTTP context
  - `MultiTenantService`: Manages current company context

### 4. API Layer (`Khidmah_Inventory.API`)

**Purpose**: Contains the web API entry point.

**Components**:
- **Controllers**: API endpoints
- **Middleware**: Custom middleware (Exception handling, Multi-tenant)
- **Configuration**: Startup, dependency injection

**Key Features**:
- JWT Authentication
- Swagger/OpenAPI documentation
- Exception handling middleware
- Multi-tenant middleware
- CORS configuration

## Data Flow

### Request Flow

```
HTTP Request
    ↓
Controller (API Layer)
    ↓
MediatR (Application Layer)
    ↓
Command/Query Handler (Application Layer)
    ↓
Repository/DbContext (Infrastructure Layer)
    ↓
Database
```

### Example: Creating a Product

1. **Controller** (`ProductsController.cs`):
   ```csharp
   [HttpPost]
   public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
   {
       var result = await Mediator.Send(command);
       return Ok(result);
   }
   ```

2. **Command** (`CreateProductCommand.cs`):
   ```csharp
   public class CreateProductCommand : IRequest<Result<ProductDto>>
   {
       public string Name { get; set; }
       // ...
   }
   ```

3. **Handler** (`CreateProductCommandHandler.cs`):
   ```csharp
   public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
   {
       private readonly IApplicationDbContext _context;
       
       public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
       {
           var product = new Product(...);
           _context.Products.Add(product);
           await _context.SaveChangesAsync(cancellationToken);
           return Result<ProductDto>.Success(mappedDto);
       }
   }
   ```

4. **Interceptor** (Automatic):
   - `AuditableEntitySaveChangesInterceptor` sets `CreatedAt`, `CreatedBy`
   - `MultiTenantInterceptor` sets `CompanyId`

## Multi-Tenancy

### Implementation

1. **All entities** inherit from `BaseEntity` which includes `CompanyId`
2. **MultiTenantInterceptor** automatically sets `CompanyId` for new entities
3. **Query filters** can be applied to filter by `CompanyId` (implemented in repositories or query handlers)
4. **Current company** is determined from:
   - `X-Company-Id` HTTP header
   - JWT token claim (`CompanyId`)

### Data Isolation

- All queries should filter by `CompanyId`
- Users can belong to multiple companies via `UserCompany` table
- Each user has a default company

## Authentication & Authorization

### Authentication

- **JWT Bearer Tokens**: Used for authentication
- **Token Claims**: Include UserId, Email, CompanyId, Roles, Permissions
- **Refresh Tokens**: Stored in User entity for token refresh

### Authorization

- **Role-Based Access Control (RBAC)**: Users have roles, roles have permissions
- **Permission-Based**: Fine-grained control via permissions
- **Attributes**: `[Authorize]`, `[Authorize(Roles = "Admin")]`, `[Authorize(Policy = "Permission")]`

## Database Design

### Key Tables

- **Users**: User accounts
- **Companies**: Multi-tenant companies
- **UserCompanies**: Many-to-many relationship between users and companies
- **Roles**: User roles
- **Permissions**: Available permissions
- **RolePermissions**: Many-to-many relationship between roles and permissions
- **UserRoles**: Many-to-many relationship between users and roles
- **Products**: Product catalog
- **Warehouses**: Storage locations
- **StockLevels**: Current stock quantities
- **StockTransactions**: Stock movement history
- **PurchaseOrders**: Purchase orders
- **SalesOrders**: Sales orders

### Conventions

- All tables have `Id` (Guid, Primary Key)
- All tables have `CompanyId` (Guid, Foreign Key)
- All tables have audit fields: `CreatedAt`, `UpdatedAt`, `CreatedBy`, `UpdatedBy`
- All tables support soft delete: `IsDeleted`, `DeletedAt`, `DeletedBy`

## Best Practices

### Adding a New Feature

1. **Domain**: Create entity in `Domain/Entities/`
2. **Application**: 
   - Create command/query in `Application/Features/[Feature]/`
   - Create handler, validator, DTOs
3. **Infrastructure**: 
   - Create entity configuration in `Infrastructure/Data/Configurations/`
   - Add DbSet to `ApplicationDbContext`
4. **API**: Create controller in `API/Controllers/`

### CQRS Guidelines

- **Commands**: Use for write operations (Create, Update, Delete)
- **Queries**: Use for read operations (Get, List, Search)
- **One Handler per Command/Query**: Keep handlers focused
- **Validation**: Use FluentValidation in validators
- **Mapping**: Use AutoMapper for entity-to-DTO mapping

### Error Handling

- Use `Result<T>` pattern for operation results
- Throw `ValidationException` for validation errors
- Use exception handling middleware for unhandled exceptions
- Return appropriate HTTP status codes

## Testing Strategy

### Unit Tests
- Test domain entities and value objects
- Test command/query handlers
- Test validators

### Integration Tests
- Test API endpoints
- Test database operations
- Test multi-tenancy isolation

### E2E Tests
- Test complete user workflows
- Test authentication/authorization

## Deployment

### Environment Variables

- `ConnectionStrings:DefaultConnection`: Database connection string
- `JwtSettings:SecretKey`: JWT signing key
- `JwtSettings:Issuer`: JWT issuer
- `JwtSettings:Audience`: JWT audience
- `JwtSettings:ExpiryMinutes`: Token expiry in minutes

### Database Migrations

```bash
dotnet ef migrations add InitialCreate --project Khidmah_Inventory.Infrastructure --startup-project Khidmah_Inventory.API
dotnet ef database update --project Khidmah_Inventory.Infrastructure --startup-project Khidmah_Inventory.API
```

## Future Enhancements

- [ ] Add SignalR for real-time notifications
- [ ] Add Hangfire/Quartz for background jobs
- [ ] Add Redis for caching
- [ ] Add Elasticsearch for advanced search
- [ ] Add file storage service (Azure Blob, S3, or MinIO)
- [ ] Add email service (SendGrid, SMTP)
- [ ] Add PDF generation service
- [ ] Add audit logging service (Serilog with sinks)
- [ ] Add API rate limiting
- [ ] Add health checks

