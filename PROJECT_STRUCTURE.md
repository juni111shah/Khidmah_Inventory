# Project Structure

## Solution Projects

```
Khidmah_Inventory.sln
├── Khidmah_Inventory.Domain/          # Domain Layer
├── Khidmah_Inventory.Application/      # Application Layer (CQRS)
├── Khidmah_Inventory.Infrastructure/   # Infrastructure Layer
├── Khidmah_Inventory.API/             # API Layer (Controllers)
└── khidmah_inventory.client/          # Angular Frontend
```

## Domain Layer (`Khidmah_Inventory.Domain`)

### Common
- `BaseEntity.cs` - Base class for all entities (Id, CompanyId, Audit fields, Soft delete)
- `Entity.cs` - Entity with domain events support
- `ValueObject.cs` - Base class for value objects
- `IDomainEvent.cs` - Interface for domain events
- `IAuditable.cs` - Interface for audit tracking
- `ISoftDeletable.cs` - Interface for soft delete
- `IMultiTenant.cs` - Interface for multi-tenancy

### Entities
- **Auth & Users**:
  - `User.cs` - User accounts
  - `Role.cs` - User roles
  - `Permission.cs` - System permissions
  - `UserRole.cs` - User-Role relationship
  - `RolePermission.cs` - Role-Permission relationship

- **Company**:
  - `Company.cs` - Multi-tenant companies
  - `UserCompany.cs` - User-Company relationship

- **Products**:
  - `Product.cs` - Products
  - `Category.cs` - Product categories (hierarchical)
  - `Brand.cs` - Product brands
  - `UnitOfMeasure.cs` - Units of measure
  - `ProductVariant.cs` - Product variants (size, color, etc.)
  - `ProductImage.cs` - Product images

- **Warehouse**:
  - `Warehouse.cs` - Warehouses
  - `WarehouseZone.cs` - Warehouse zones
  - `Bin.cs` - Storage bins/locations

- **Inventory**:
  - `StockTransaction.cs` - Stock movement transactions
  - `StockLevel.cs` - Current stock levels

- **Purchase**:
  - `Supplier.cs` - Suppliers
  - `PurchaseOrder.cs` - Purchase orders
  - `PurchaseOrderItem.cs` - Purchase order line items

- **Sales**:
  - `Customer.cs` - Customers
  - `SalesOrder.cs` - Sales orders
  - `SalesOrderItem.cs` - Sales order line items

## Application Layer (`Khidmah_Inventory.Application`)

### Common
- `Interfaces/`:
  - `IApplicationDbContext.cs` - Database context interface
  - `ICurrentUserService.cs` - Current user service interface
  - `IMultiTenantService.cs` - Multi-tenant service interface
  - `IIdentityService.cs` - Identity service interface

- `Models/`:
  - `Result.cs` - Result pattern for operations
  - `PagedResult.cs` - Paginated results

- `Behaviors/`:
  - `ValidationBehavior.cs` - MediatR pipeline behavior for validation

- `Mappings/`:
  - `MappingProfile.cs` - AutoMapper configuration

### Features (CQRS)
- `Features/Auth/`:
  - `Commands/Login/`:
    - `LoginCommand.cs`
    - `LoginCommandHandler.cs`
    - `LoginCommandValidator.cs`
  - `Commands/Register/`:
    - `RegisterCommand.cs`
    - `RegisterCommandHandler.cs`
    - `RegisterCommandValidator.cs`

## Infrastructure Layer (`Khidmah_Inventory.Infrastructure`)

### Data
- `ApplicationDbContext.cs` - EF Core database context
- `Interceptors/`:
  - `AuditableEntitySaveChangesInterceptor.cs` - Auto-sets audit fields
  - `MultiTenantInterceptor.cs` - Auto-sets CompanyId
- `Configurations/`:
  - `UserConfiguration.cs` - User entity configuration
  - `CompanyConfiguration.cs` - Company entity configuration
  - `ProductConfiguration.cs` - Product entity configuration
  - `WarehouseConfiguration.cs` - Warehouse entity configuration
  - `StockLevelConfiguration.cs` - StockLevel entity configuration
- `Extensions/`:
  - `ModelBuilderExtensions.cs` - EF Core model builder extensions

### Services
- `IdentityService.cs` - Password hashing, JWT token generation
- `CurrentUserService.cs` - Gets current user from HTTP context
- `MultiTenantService.cs` - Manages current company context

### Dependency Injection
- `DependencyInjection.cs` - Service registration

## API Layer (`Khidmah_Inventory.API`)

### Controllers
- `BaseApiController.cs` - Base controller with MediatR
- `AuthController.cs` - Authentication endpoints
- `ProductsController.cs` - Product endpoints (placeholder)
- `CompaniesController.cs` - Company endpoints (placeholder)

### Middleware
- `MultiTenantMiddleware.cs` - Extracts company ID from headers
- `ExceptionHandlingMiddleware.cs` - Global exception handling

### Configuration
- `Program.cs` - Application startup and configuration
- `appsettings.json` - Application settings
- `appsettings.Development.json` - Development settings
- `Properties/launchSettings.json` - Launch profiles

## Code Flow Example

### Creating a Product

1. **API Controller** (`ProductsController.cs`):
   ```csharp
   [HttpPost]
   public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
   {
       var result = await Mediator.Send(command);
       return Ok(result);
   }
   ```

2. **Application Command** (`CreateProductCommand.cs`):
   ```csharp
   public class CreateProductCommand : IRequest<Result<ProductDto>>
   {
       public string Name { get; set; }
       public string SKU { get; set; }
       // ...
   }
   ```

3. **Application Handler** (`CreateProductCommandHandler.cs`):
   ```csharp
   public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
   {
       private readonly IApplicationDbContext _context;
       
       public async Task<Result<ProductDto>> Handle(...)
       {
           var product = new Product(companyId, name, sku, ...);
           _context.Products.Add(product);
           await _context.SaveChangesAsync(cancellationToken);
           // Map to DTO and return
       }
   }
   ```

4. **Infrastructure Interceptors** (Automatic):
   - Sets `CreatedAt`, `CreatedBy` (AuditableEntitySaveChangesInterceptor)
   - Sets `CompanyId` (MultiTenantInterceptor)

5. **Database** (EF Core):
   - Saves entity to database

## Next Steps

To complete the implementation, you need to:

1. **Add more CQRS commands/queries** for each module:
   - Products: Create, Update, Delete, Get, List
   - Companies: Create, Update, Get, List
   - Warehouses: CRUD operations
   - Inventory: Stock In, Stock Out, Adjustments
   - Purchase: Create PO, Receive GRN
   - Sales: Create SO, Deliver, Invoice

2. **Add more entity configurations** for all entities

3. **Add repository pattern** (optional, if needed for complex queries)

4. **Add authorization policies** for permissions

5. **Add SignalR** for real-time notifications

6. **Add background jobs** (Hangfire/Quartz) for scheduled tasks

7. **Add file storage service** for images and documents

8. **Add PDF generation** for invoices, PO, SO

9. **Add email service** for notifications

10. **Add audit logging** with Serilog

## File Organization Best Practices

- **One feature per folder**: All related commands/queries/handlers in one folder
- **Separate commands and queries**: Clear separation of read and write operations
- **Validators with commands**: Each command has its own validator
- **DTOs in same folder**: DTOs are co-located with commands/queries
- **Entity configurations**: One configuration file per entity
- **Services grouped by concern**: Identity, Multi-tenant, etc.

