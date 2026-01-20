# Brands Module Implementation

## Feature Overview

The Brands module manages product brands. Brands are simple entities that categorize products by manufacturer or brand name.

## Business Requirements

1. **Brand Management**
   - Create, read, update, and delete brands
   - Brand name (required, unique within company)
   - Optional description
   - Logo support
   - Website URL

2. **Brand Attributes**
   - Name (required)
   - Description (optional)
   - Logo URL
   - Website URL

## Domain Model

### Entity: Brand
- **Location**: `Khidmah_Inventory.Domain/Entities/Brand.cs`
- **Relationships**:
  - Has many: Products

### Key Properties
- `Name`, `Description`
- `LogoUrl`, `Website`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Brand Command
**File**: `Khidmah_Inventory.Application/Features/Brands/Commands/CreateBrand/CreateBrandCommand.cs`

```csharp
public class CreateBrandCommand : IRequest<Result<BrandDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Website { get; set; }
}
```

**Handler**: 
- Validate name is unique within company
- Create Brand entity
- Save to database
- Return BrandDto

**Validator**:
```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
RuleFor(x => x.Website).Must(BeValidUrl).When(x => !string.IsNullOrEmpty(x.Website));
```

#### Update Brand Command
- Validate name uniqueness (excluding current brand)
- Update entity
- Save changes

#### Delete Brand Command
- Check if brand has products
- Soft delete if no products, or prevent deletion

### 2. Application Layer - Queries

#### Get Brand Query
Load brand with product count.

#### Get Brands List Query
**File**: `Khidmah_Inventory.Application/Features/Brands/Queries/GetBrandsList/GetBrandsListQuery.cs`

```csharp
public class GetBrandsListQuery : IRequest<Result<List<BrandDto>>>
{
    public string? SearchTerm { get; set; }
}
```

**Handler**:
- Filter by CompanyId
- Apply search term if provided
- Order by name
- Return list

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/Brands/BrandDto.cs`

```csharp
public class BrandDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public string? Website { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/BrandConfiguration.cs`

```csharp
public class BrandConfiguration : IEntityTypeConfiguration<Brand>
{
    public void Configure(EntityTypeBuilder<Brand> builder)
    {
        builder.HasKey(b => b.Id);
        
        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(b => b.Website)
            .HasMaxLength(500);
            
        // Indexes
        builder.HasIndex(b => new { b.CompanyId, b.Name })
            .IsUnique();
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/BrandsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BrandsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBrandCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetBrandQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetBrandsListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBrandCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteBrandCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result);
    }
    
    [HttpPost("{id}/logo")]
    public async Task<IActionResult> UploadLogo(Guid id, IFormFile file)
    {
        var command = new UploadBrandLogoCommand { Id = id, File = file };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/brands` | Create brand | Required |
| GET | `/api/brands/{id}` | Get brand by ID | Required |
| GET | `/api/brands` | Get brands list | Required |
| PUT | `/api/brands/{id}` | Update brand | Required |
| DELETE | `/api/brands/{id}` | Delete brand | Required |
| POST | `/api/brands/{id}/logo` | Upload brand logo | Required |

## Frontend Components

### 1. Brand Service
**File**: `khidmah_inventory.client/src/app/core/services/brand-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class BrandApiService {
  private apiUrl = '/api/brands';

  constructor(private http: HttpClient) {}

  getBrands(searchTerm?: string): Observable<BrandDto[]> {
    const params = searchTerm ? { searchTerm } : {};
    return this.http.get<BrandDto[]>(this.apiUrl, { params });
  }

  getBrand(id: string): Observable<BrandDto> {
    return this.http.get<BrandDto>(`${this.apiUrl}/${id}`);
  }

  createBrand(brand: CreateBrandDto): Observable<BrandDto> {
    return this.http.post<BrandDto>(this.apiUrl, brand);
  }

  updateBrand(id: string, brand: UpdateBrandDto): Observable<BrandDto> {
    return this.http.put<BrandDto>(`${this.apiUrl}/${id}`, brand);
  }

  deleteBrand(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  uploadLogo(id: string, file: File): Observable<BrandDto> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<BrandDto>(`${this.apiUrl}/${id}/logo`, formData);
  }
}
```

### 2. Brand List Component
- Display brands in table/grid
- Search functionality
- Logo display
- Actions: View, Edit, Delete

### 3. Brand Form Component
- Create/Edit brand form
- Logo upload
- Website URL validation
- Validation

## Workflow

### Create Brand Flow
```
User fills form → Validate name uniqueness → Submit
→ API → Command Handler → Create entity → Save
→ Return DTO → Update list
```

## Testing Checklist

- [ ] Create brand with valid data
- [ ] Create brand with duplicate name (should fail)
- [ ] Update brand
- [ ] Delete brand with products (should fail or warn)
- [ ] Delete brand without products
- [ ] Get brands list
- [ ] Search brands
- [ ] Upload brand logo
- [ ] Multi-tenancy isolation
- [ ] Frontend: Brand list displays correctly
- [ ] Frontend: Logo upload works

