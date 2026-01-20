# Categories Module Implementation

## Feature Overview

The Categories module manages hierarchical product categories, allowing products to be organized in a tree structure. Categories support parent-child relationships for flexible organization.

## Business Requirements

1. **Category Management**
   - Create, read, update, and delete categories
   - Support hierarchical structure (parent-child)
   - Display order for sorting
   - Category code for identification
   - Image support

2. **Hierarchical Structure**
   - Categories can have parent categories
   - Categories can have subcategories
   - Prevent circular references
   - Support unlimited depth

3. **Category Attributes**
   - Name (required)
   - Code (optional, unique within company)
   - Description
   - Display order
   - Image URL

## Domain Model

### Entity: Category
- **Location**: `Khidmah_Inventory.Domain/Entities/Category.cs`
- **Relationships**:
  - Self-referencing: ParentCategory → SubCategories
  - Has many: Products

### Key Properties
- `Name`, `Code`, `Description`
- `ParentCategoryId` (nullable for root categories)
- `DisplayOrder`
- `ImageUrl`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Category Command
**File**: `Khidmah_Inventory.Application/Features/Categories/Commands/CreateCategory/CreateCategoryCommand.cs`

```csharp
public class CreateCategoryCommand : IRequest<Result<CategoryDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; } = 0;
}
```

**Handler**: 
- Validate name is unique within company and parent
- Validate code is unique if provided
- Validate parent exists and belongs to same company
- Prevent circular reference
- Create Category entity
- Save to database

**Validator**:
```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
RuleFor(x => x.Code).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Code));
```

#### Update Category Command
- Similar to create
- Prevent moving category to its own descendant (circular reference check)

#### Delete Category Command
- Check if category has products
- Check if category has subcategories
- Soft delete if allowed, or prevent deletion

### 2. Application Layer - Queries

#### Get Category Query
Load category with parent and subcategories.

#### Get Categories Tree Query
**File**: `Khidmah_Inventory.Application/Features/Categories/Queries/GetCategoriesTree/GetCategoriesTreeQuery.cs`

```csharp
public class GetCategoriesTreeQuery : IRequest<Result<List<CategoryTreeNodeDto>>>
{
    public Guid? ParentCategoryId { get; set; } // null for root level
}
```

**Handler**:
- Load categories filtered by CompanyId and ParentCategoryId
- Order by DisplayOrder
- Return tree structure

#### Get Categories List Query
Flat list with parent information for dropdowns.

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/Categories/CategoryDto.cs`

```csharp
public class CategoryDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public string? ParentCategoryName { get; set; }
    public int DisplayOrder { get; set; }
    public string? ImageUrl { get; set; }
    public int ProductCount { get; set; }
    public int SubCategoryCount { get; set; }
    public List<CategoryDto> SubCategories { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class CategoryTreeNodeDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; }
    public int ProductCount { get; set; }
    public List<CategoryTreeNodeDto> Children { get; set; } = new();
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/CategoryConfiguration.cs`

```csharp
public class CategoryConfiguration : IEntityTypeConfiguration<Category>
{
    public void Configure(EntityTypeBuilder<Category> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(c => c.Code)
            .HasMaxLength(50);
            
        // Indexes
        builder.HasIndex(c => new { c.CompanyId, c.ParentCategoryId, c.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL");
            
        // Self-referencing relationship
        builder.HasOne(c => c.ParentCategory)
            .WithMany(c => c.SubCategories)
            .HasForeignKey(c => c.ParentCategoryId)
            .OnDelete(DeleteBehavior.Restrict); // Prevent cascade delete
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/CategoriesController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CategoriesController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCategoryCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetCategoryQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet("tree")]
    public async Task<IActionResult> GetTree([FromQuery] Guid? parentId)
    {
        var query = new GetCategoriesTreeQuery { ParentCategoryId = parentId };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var query = new GetCategoriesListQuery();
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCategoryCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/categories` | Create category | Required |
| GET | `/api/categories/{id}` | Get category by ID | Required |
| GET | `/api/categories/tree` | Get categories tree | Required |
| GET | `/api/categories` | Get categories list | Required |
| PUT | `/api/categories/{id}` | Update category | Required |
| DELETE | `/api/categories/{id}` | Delete category | Required |

## Frontend Components

### 1. Category Service
**File**: `khidmah_inventory.client/src/app/core/services/category-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class CategoryApiService {
  private apiUrl = '/api/categories';

  constructor(private http: HttpClient) {}

  getCategories(): Observable<CategoryDto[]> {
    return this.http.get<CategoryDto[]>(this.apiUrl);
  }

  getCategoryTree(parentId?: string): Observable<CategoryTreeNodeDto[]> {
    const params = parentId ? { parentId } : {};
    return this.http.get<CategoryTreeNodeDto[]>(`${this.apiUrl}/tree`, { params });
  }

  getCategory(id: string): Observable<CategoryDto> {
    return this.http.get<CategoryDto>(`${this.apiUrl}/${id}`);
  }

  createCategory(category: CreateCategoryDto): Observable<CategoryDto> {
    return this.http.post<CategoryDto>(this.apiUrl, category);
  }

  updateCategory(id: string, category: UpdateCategoryDto): Observable<CategoryDto> {
    return this.http.put<CategoryDto>(`${this.apiUrl}/${id}`, category);
  }

  deleteCategory(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

### 2. Category Tree Component
- Display hierarchical tree structure
- Expand/collapse nodes
- Drag and drop for reordering (optional)
- Context menu for actions

### 3. Category Form Component
- Create/Edit category form
- Parent category dropdown (tree selector)
- Image upload
- Display order input

## Workflow

### Create Category Flow
```
User fills form → Select parent (optional) → Validate
→ Check circular reference → Create entity → Save
→ Return DTO → Update tree view
```

### Delete Category Flow
```
User clicks delete → Check for products → Check for subcategories
→ If safe, soft delete → Update tree view
→ If not safe, show error message
```

## Testing Checklist

- [ ] Create root category
- [ ] Create subcategory
- [ ] Prevent circular reference (moving category under its descendant)
- [ ] Prevent duplicate name within same parent
- [ ] Prevent duplicate code within company
- [ ] Delete category with products (should fail or warn)
- [ ] Delete category with subcategories (should fail or warn)
- [ ] Get categories tree
- [ ] Update category parent
- [ ] Multi-tenancy isolation
- [ ] Frontend: Tree display works correctly
- [ ] Frontend: Parent selection works

