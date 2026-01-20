# Implementation Guide

## Overview

This guide provides a comprehensive roadmap for implementing all modules in the Khidmah Inventory Management System. The system follows Clean Architecture principles with CQRS pattern using MediatR.

## Current Implementation Status

### ✅ Fully Implemented
- **Authentication Module** (Backend only - needs frontend integration)
  - Login and Register endpoints
  - JWT token generation
  - Multi-tenant user-company relationships
  - Role and permission retrieval

- **Theme Module** (Complete end-to-end)
  - Theme management API
  - Frontend theme service and components
  - Logo upload functionality

### 🚧 Partially Implemented
- **Products Module** - Controller placeholder exists, no implementation
- **Companies Module** - Controller placeholder exists, no implementation

### ❌ Not Implemented
All other modules need complete implementation from domain to frontend.

## Architecture Patterns

### 1. Clean Architecture Layers

```
┌─────────────────────────────────────┐
│         API Layer                   │
│    (Controllers, Middleware)         │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Application Layer               │
│  (Commands, Queries, Handlers)      │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│      Domain Layer                    │
│   (Entities, Value Objects)         │
└──────────────┬──────────────────────┘
               │
┌──────────────▼──────────────────────┐
│    Infrastructure Layer              │
│  (DbContext, Services, Configs)      │
└──────────────────────────────────────┘
```

### 2. CQRS Pattern

- **Commands**: Write operations (Create, Update, Delete)
- **Queries**: Read operations (Get, List, Search)
- **MediatR**: Dispatches commands/queries to handlers

### 3. Multi-Tenancy

- All entities inherit from `BaseEntity` with `CompanyId`
- `MultiTenantInterceptor` automatically sets `CompanyId`
- All queries must filter by `CompanyId`

### 4. Result Pattern

All handlers return `Result<T>` for consistent error handling:
```csharp
Result<T>.Success(data)
Result<T>.Failure("Error message")
```

## Implementation Workflow

### Step 1: Domain Layer (if needed)
- Review entity in `Domain/Entities/`
- Add domain methods if needed
- Ensure entity follows domain patterns

### Step 2: Application Layer

#### Create Commands/Queries Structure
```
Features/
  [ModuleName]/
    Commands/
      Create[Entity]/
        Create[Entity]Command.cs
        Create[Entity]CommandHandler.cs
        Create[Entity]CommandValidator.cs
      Update[Entity]/
        ...
      Delete[Entity]/
        ...
    Queries/
      Get[Entity]/
        Get[Entity]Query.cs
        Get[Entity]QueryHandler.cs
      Get[Entity]List/
        ...
```

#### Command Example
```csharp
public class CreateProductCommand : IRequest<Result<ProductDto>>
{
    public string Name { get; set; }
    public string SKU { get; set; }
    // ... other properties
}

public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    
    public async Task<Result<ProductDto>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var product = new Product(
            _currentUser.CompanyId,
            request.Name,
            request.SKU,
            // ... other parameters
        );
        
        _context.Products.Add(product);
        await _context.SaveChangesAsync(cancellationToken);
        
        return Result<ProductDto>.Success(mappedDto);
    }
}
```

#### Query Example
```csharp
public class GetProductQuery : IRequest<Result<ProductDto>>
{
    public Guid Id { get; set; }
}

public class GetProductQueryHandler : IRequestHandler<GetProductQuery, Result<ProductDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    
    public async Task<Result<ProductDto>> Handle(GetProductQuery request, CancellationToken cancellationToken)
    {
        var product = await _context.Products
            .Where(p => p.Id == request.Id && p.CompanyId == _currentUser.CompanyId)
            .FirstOrDefaultAsync(cancellationToken);
            
        if (product == null)
            return Result<ProductDto>.Failure("Product not found");
            
        return Result<ProductDto>.Success(mappedDto);
    }
}
```

### Step 3: Infrastructure Layer

#### Entity Configuration (if needed)
Create in `Infrastructure/Data/Configurations/`:
```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        builder.Property(p => p.Name).IsRequired().HasMaxLength(200);
        // ... other configurations
    }
}
```

### Step 4: API Layer

#### Controller Implementation
```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class ProductsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
    {
        var result = await Mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result.Data);
            
        return BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetProductQuery { Id = id };
        var result = await Mediator.Send(query);
        
        if (result.Succeeded)
            return Ok(result.Data);
            
        return NotFound(result);
    }
}
```

### Step 5: Frontend Integration

#### Create Service
```typescript
@Injectable({ providedIn: 'root' })
export class ProductApiService {
  private apiUrl = '/api/products';

  constructor(private http: HttpClient) {}

  getProducts(): Observable<ProductDto[]> {
    return this.http.get<ProductDto[]>(this.apiUrl);
  }

  getProduct(id: string): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/${id}`);
  }

  createProduct(product: CreateProductDto): Observable<ProductDto> {
    return this.http.post<ProductDto>(this.apiUrl, product);
  }
}
```

#### Create Component
```typescript
@Component({
  selector: 'app-products',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './products.component.html'
})
export class ProductsComponent {
  products: ProductDto[] = [];

  constructor(private productService: ProductApiService) {}

  ngOnInit() {
    this.loadProducts();
  }

  loadProducts() {
    this.productService.getProducts().subscribe({
      next: (data) => this.products = data,
      error: (err) => console.error(err)
    });
  }
}
```

## Common Patterns

### Validation
- Use FluentValidation for command validation
- Create validators in same folder as command
- Register validators in DI container

### Mapping
- Use AutoMapper for entity-to-DTO mapping
- Create mapping profiles in `Application/Common/Mappings/`

### Error Handling
- Use `Result<T>` pattern
- Return appropriate HTTP status codes
- Use exception handling middleware for unhandled exceptions

### Multi-Tenancy
- Always filter by `CompanyId` in queries
- Use `ICurrentUserService` to get current company
- Interceptors handle automatic `CompanyId` setting

### Audit Fields
- `AuditableEntitySaveChangesInterceptor` automatically sets:
  - `CreatedAt`, `CreatedBy` (on create)
  - `UpdatedAt`, `UpdatedBy` (on update)
- No manual setting needed

## Implementation Priority

Follow this order to respect dependencies:

### Phase 1: Foundation
1. Companies
2. Units of Measure
3. Categories
4. Brands

### Phase 2: Core Inventory
5. Products
6. Warehouses
7. Warehouse Zones
8. Bins

### Phase 3: Inventory Tracking
9. Stock Levels
10. Stock Transactions

### Phase 4: Business Partners
11. Suppliers
12. Customers

### Phase 5: Transactions
13. Purchase Orders
14. Sales Orders

### Phase 6: User Management
15. Users (extend auth)
16. Roles & Permissions

### Phase 7: Frontend
17. Auth Frontend Integration
18. All module UIs

## Testing Guidelines

### Unit Tests
- Test command/query handlers
- Test validators
- Test domain entity methods

### Integration Tests
- Test API endpoints
- Test database operations
- Test multi-tenancy isolation

### Frontend Tests
- Test services
- Test components
- Test user interactions

## Module Documentation

Each module has detailed implementation documentation in `docs/implementation/`:

- `01_PRODUCTS.md` - Product management
- `02_CATEGORIES.md` - Category management
- `03_BRANDS.md` - Brand management
- `04_UNITS_OF_MEASURE.md` - Unit of measure
- `05_COMPANIES.md` - Company management
- `06_USERS.md` - User management
- `07_ROLES_PERMISSIONS.md` - RBAC
- `08_WAREHOUSES.md` - Warehouse management
- `09_WAREHOUSE_ZONES.md` - Zone management
- `10_BINS.md` - Bin locations
- `11_STOCK_LEVELS.md` - Stock tracking
- `12_STOCK_TRANSACTIONS.md` - Stock history
- `13_SUPPLIERS.md` - Supplier management
- `14_PURCHASE_ORDERS.md` - Purchase orders
- `15_CUSTOMERS.md` - Customer management
- `16_SALES_ORDERS.md` - Sales orders
- `17_AUTH_FRONTEND.md` - Frontend auth
- `18_FRONTEND_PATTERNS.md` - Frontend patterns

## Next Steps

1. Review the module-specific documentation
2. Start with Phase 1 (Foundation modules)
3. Follow the implementation workflow for each module
4. Test thoroughly before moving to next module
5. Update this guide as you complete modules

