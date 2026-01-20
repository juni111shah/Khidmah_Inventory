# Products Module Implementation

## Feature Overview

The Products module manages the product catalog, including product information, variants, images, pricing, and inventory settings. Products are the core entities in the inventory system and are referenced by stock levels, purchase orders, and sales orders.

## Business Requirements

1. **Product Management**
   - Create, read, update, and delete products
   - Support product variants (size, color, etc.)
   - Manage product images
   - Track product pricing (purchase, sale, cost)
   - Set inventory thresholds (min, max, reorder point)

2. **Product Attributes**
   - SKU (Stock Keeping Unit) - unique identifier
   - Barcode support
   - Category and brand association
   - Unit of measure
   - Physical dimensions and weight
   - Batch and expiry tracking options

3. **Product Status**
   - Active/Inactive status
   - Soft delete support

## Domain Model

### Entity: Product
- **Location**: `Khidmah_Inventory.Domain/Entities/Product.cs`
- **Relationships**:
  - Belongs to: Category (optional), Brand (optional), UnitOfMeasure (required)
  - Has many: ProductVariant, ProductImage, StockTransaction

### Key Properties
- `Name`, `SKU`, `Barcode`
- `CategoryId`, `BrandId`, `UnitOfMeasureId`
- `PurchasePrice`, `SalePrice`, `CostPrice`
- `MinStockLevel`, `MaxStockLevel`, `ReorderPoint`
- `TrackQuantity`, `TrackBatch`, `TrackExpiry`
- `IsActive`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Product Command
**File**: `Khidmah_Inventory.Application/Features/Products/Commands/CreateProduct/CreateProductCommand.cs`

```csharp
public class CreateProductCommand : IRequest<Result<ProductDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public decimal? ReorderPoint { get; set; }
    public bool TrackQuantity { get; set; } = true;
    public bool TrackBatch { get; set; } = false;
    public bool TrackExpiry { get; set; } = false;
    public decimal Weight { get; set; }
    public string? WeightUnit { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? DimensionsUnit { get; set; }
}
```

**Handler**: `CreateProductCommandHandler.cs`
- Validate SKU uniqueness within company
- Validate UnitOfMeasure exists
- Validate Category and Brand if provided
- Create Product entity
- Generate barcode if not provided
- Save to database
- Return ProductDto

**Validator**: `CreateProductCommandValidator.cs`
```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
RuleFor(x => x.SKU).NotEmpty().MaximumLength(50);
RuleFor(x => x.UnitOfMeasureId).NotEmpty();
RuleFor(x => x.PurchasePrice).GreaterThanOrEqualTo(0);
RuleFor(x => x.SalePrice).GreaterThanOrEqualTo(0);
```

#### Update Product Command
Similar structure with `Id` property and update logic.

#### Delete Product Command
- Soft delete (set IsDeleted flag)
- Check if product has stock transactions
- Check if product is referenced in active orders

#### Activate/Deactivate Product Commands
Simple commands to toggle `IsActive` status.

### 2. Application Layer - Queries

#### Get Product Query
**File**: `Khidmah_Inventory.Application/Features/Products/Queries/GetProduct/GetProductQuery.cs`

```csharp
public class GetProductQuery : IRequest<Result<ProductDto>>
{
    public Guid Id { get; set; }
}
```

**Handler**: 
- Load product with includes (Category, Brand, UnitOfMeasure, Variants, Images)
- Filter by CompanyId
- Return ProductDto

#### Get Products List Query
**File**: `Khidmah_Inventory.Application/Features/Products/Queries/GetProductsList/GetProductsListQuery.cs`

```csharp
public class GetProductsListQuery : IRequest<Result<PagedResult<ProductDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public bool? IsActive { get; set; }
    public string? SortBy { get; set; }
    public bool SortDescending { get; set; } = false;
}
```

**Handler**:
- Apply filters (CompanyId, SearchTerm, CategoryId, BrandId, IsActive)
- Apply sorting
- Apply pagination
- Return paged results

#### Search Products Query
For autocomplete/search functionality with minimal data.

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/Products/ProductDto.cs`

```csharp
public class ProductDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string SKU { get; set; } = string.Empty;
    public string? Barcode { get; set; }
    public Guid? CategoryId { get; set; }
    public string? CategoryName { get; set; }
    public Guid? BrandId { get; set; }
    public string? BrandName { get; set; }
    public Guid UnitOfMeasureId { get; set; }
    public string UnitOfMeasureName { get; set; } = string.Empty;
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public decimal? CostPrice { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public decimal? ReorderPoint { get; set; }
    public bool TrackQuantity { get; set; }
    public bool TrackBatch { get; set; }
    public bool TrackExpiry { get; set; }
    public bool IsActive { get; set; }
    public string? ImageUrl { get; set; }
    public decimal Weight { get; set; }
    public string? WeightUnit { get; set; }
    public decimal? Length { get; set; }
    public decimal? Width { get; set; }
    public decimal? Height { get; set; }
    public string? DimensionsUnit { get; set; }
    public List<ProductVariantDto> Variants { get; set; } = new();
    public List<ProductImageDto> Images { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. Mapping

**File**: `Khidmah_Inventory.Application/Common/Mappings/ProductMappingProfile.cs`

```csharp
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(d => d.CategoryName, opt => opt.MapFrom(s => s.Category != null ? s.Category.Name : null))
            .ForMember(d => d.BrandName, opt => opt.MapFrom(s => s.Brand != null ? s.Brand.Name : null))
            .ForMember(d => d.UnitOfMeasureName, opt => opt.MapFrom(s => s.UnitOfMeasure.Name));
            
        CreateMap<CreateProductCommand, Product>()
            .ForMember(d => d.Id, opt => opt.Ignore())
            .ForMember(d => d.Category, opt => opt.Ignore())
            .ForMember(d => d.Brand, opt => opt.Ignore())
            .ForMember(d => d.UnitOfMeasure, opt => opt.Ignore());
    }
}
```

### 5. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/ProductConfiguration.cs`

```csharp
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.HasKey(p => p.Id);
        
        builder.Property(p => p.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(p => p.SKU)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(p => p.Barcode)
            .HasMaxLength(100);
            
        builder.Property(p => p.PurchasePrice)
            .HasPrecision(18, 2);
            
        builder.Property(p => p.SalePrice)
            .HasPrecision(18, 2);
            
        // Indexes
        builder.HasIndex(p => new { p.CompanyId, p.SKU })
            .IsUnique();
            
        builder.HasIndex(p => p.Barcode)
            .HasFilter("[Barcode] IS NOT NULL");
            
        // Relationships
        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne(p => p.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(p => p.BrandId)
            .OnDelete(DeleteBehavior.SetNull);
            
        builder.HasOne(p => p.UnitOfMeasure)
            .WithMany()
            .HasForeignKey(p => p.UnitOfMeasureId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 6. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/ProductsController.cs`

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
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetProductsListQuery query)
    {
        var result = await Mediator.Send(query);
        
        if (result.Succeeded)
            return Ok(result.Data);
            
        return BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result.Data);
            
        return BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteProductCommand { Id = id };
        var result = await Mediator.Send(command);
        
        if (result.Succeeded)
            return NoContent();
            
        return BadRequest(result);
    }
    
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateProductCommand { Id = id };
        var result = await Mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result.Data);
            
        return BadRequest(result);
    }
    
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateProductCommand { Id = id };
        var result = await Mediator.Send(command);
        
        if (result.Succeeded)
            return Ok(result.Data);
            
        return BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/products` | Create product | Required |
| GET | `/api/products/{id}` | Get product by ID | Required |
| GET | `/api/products` | Get products list (paginated) | Required |
| PUT | `/api/products/{id}` | Update product | Required |
| DELETE | `/api/products/{id}` | Delete product (soft) | Required |
| POST | `/api/products/{id}/activate` | Activate product | Required |
| POST | `/api/products/{id}/deactivate` | Deactivate product | Required |

## Frontend Components

### 1. Product Service
**File**: `khidmah_inventory.client/src/app/core/services/product-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class ProductApiService {
  private apiUrl = '/api/products';

  constructor(private http: HttpClient) {}

  getProducts(params?: any): Observable<PagedResult<ProductDto>> {
    return this.http.get<PagedResult<ProductDto>>(this.apiUrl, { params });
  }

  getProduct(id: string): Observable<ProductDto> {
    return this.http.get<ProductDto>(`${this.apiUrl}/${id}`);
  }

  createProduct(product: CreateProductDto): Observable<ProductDto> {
    return this.http.post<ProductDto>(this.apiUrl, product);
  }

  updateProduct(id: string, product: UpdateProductDto): Observable<ProductDto> {
    return this.http.put<ProductDto>(`${this.apiUrl}/${id}`, product);
  }

  deleteProduct(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  activateProduct(id: string): Observable<ProductDto> {
    return this.http.post<ProductDto>(`${this.apiUrl}/${id}/activate`, {});
  }

  deactivateProduct(id: string): Observable<ProductDto> {
    return this.http.post<ProductDto>(`${this.apiUrl}/${id}/deactivate`, {});
  }
}
```

### 2. Product List Component
**File**: `khidmah_inventory.client/src/app/features/products/product-list/product-list.component.ts`

- Display products in table/grid
- Search and filter functionality
- Pagination
- Actions: View, Edit, Delete, Activate/Deactivate

### 3. Product Form Component
**File**: `khidmah_inventory.client/src/app/features/products/product-form/product-form.component.ts`

- Create/Edit product form
- Category and Brand dropdowns
- Unit of Measure selection
- Image upload
- Validation

### 4. Product Detail Component
**File**: `khidmah_inventory.client/src/app/features/products/product-detail/product-detail.component.ts`

- View product details
- Show variants and images
- Stock level information
- Transaction history link

## Workflow

### Create Product Flow
```
User fills form → Validate → Submit → API → Command Handler
→ Validate business rules → Create entity → Save → Return DTO
→ Update UI → Show success message
```

### Product Search Flow
```
User types in search → Debounce → API call → Query Handler
→ Filter by CompanyId and search term → Return results
→ Display in dropdown/list
```

## Testing Checklist

- [ ] Create product with valid data
- [ ] Create product with duplicate SKU (should fail)
- [ ] Create product with invalid UnitOfMeasure (should fail)
- [ ] Update product
- [ ] Delete product (soft delete)
- [ ] Activate/Deactivate product
- [ ] Get product by ID
- [ ] Get products list with pagination
- [ ] Search products by name/SKU
- [ ] Filter products by category
- [ ] Filter products by brand
- [ ] Multi-tenancy isolation (users can only see their company's products)
- [ ] Frontend: Product list displays correctly
- [ ] Frontend: Product form validation works
- [ ] Frontend: Image upload works

