# Warehouses Module Implementation

## Feature Overview

The Warehouses module manages storage locations where inventory is kept. Warehouses can have zones and bins for detailed location tracking.

## Business Requirements

1. **Warehouse Management**
   - Create, read, update, and delete warehouses
   - Warehouse code and name
   - Address and contact information
   - Default warehouse designation
   - Active/Inactive status

2. **Warehouse Attributes**
   - Name (required)
   - Code (optional, unique within company)
   - Description
   - Address (street, city, state, country, postal code)
   - Contact (phone, email)
   - IsDefault flag
   - IsActive flag

## Domain Model

### Entity: Warehouse
- **Location**: `Khidmah_Inventory.Domain/Entities/Warehouse.cs`
- **Relationships**:
  - Has many: WarehouseZones
  - Has many: StockTransactions

### Key Properties
- `Name`, `Code`, `Description`
- `Address`, `City`, `State`, `Country`, `PostalCode`
- `PhoneNumber`, `Email`
- `IsDefault`, `IsActive`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Warehouse Command
**File**: `Khidmah_Inventory.Application/Features/Warehouses/Commands/CreateWarehouse/CreateWarehouseCommand.cs`

```csharp
public class CreateWarehouseCommand : IRequest<Result<WarehouseDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsDefault { get; set; } = false;
}
```

**Handler**: 
- Validate name is unique within company
- Validate code is unique if provided
- If IsDefault, unset other default warehouses
- Create Warehouse entity
- Save to database

**Validator**:
```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
RuleFor(x => x.Code).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Code));
RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
```

#### Update Warehouse Command
- Similar validation
- Handle default warehouse change

#### Set Default Warehouse Command
- Unset current default
- Set new default

#### Delete Warehouse Command
- Check if warehouse has stock
- Check if warehouse has transactions
- Prevent deletion if in use

### 2. Application Layer - Queries

#### Get Warehouse Query
Load warehouse with zone count and stock level summary.

#### Get Warehouses List Query
**File**: `Khidmah_Inventory.Application/Features/Warehouses/Queries/GetWarehousesList/GetWarehousesListQuery.cs`

```csharp
public class GetWarehousesListQuery : IRequest<Result<List<WarehouseDto>>>
{
    public bool? IsActive { get; set; }
}
```

**Handler**:
- Filter by CompanyId
- Filter by IsActive if provided
- Order by IsDefault desc, then Name
- Return list

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/Warehouses/WarehouseDto.cs`

```csharp
public class WarehouseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Email { get; set; }
    public bool IsDefault { get; set; }
    public bool IsActive { get; set; }
    public int ZoneCount { get; set; }
    public int ProductCount { get; set; } // Products with stock in this warehouse
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/WarehouseConfiguration.cs`

```csharp
public class WarehouseConfiguration : IEntityTypeConfiguration<Warehouse>
{
    public void Configure(EntityTypeBuilder<Warehouse> builder)
    {
        builder.HasKey(w => w.Id);
        
        builder.Property(w => w.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(w => w.Code)
            .HasMaxLength(50);
            
        // Indexes
        builder.HasIndex(w => new { w.CompanyId, w.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL");
            
        builder.HasIndex(w => new { w.CompanyId, w.IsDefault })
            .HasFilter("[IsDefault] = 1");
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/WarehousesController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehousesController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetWarehouseQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetWarehousesListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteWarehouseCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result);
    }
    
    [HttpPost("{id}/set-default")]
    public async Task<IActionResult> SetDefault(Guid id)
    {
        var command = new SetDefaultWarehouseCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/warehouses` | Create warehouse | Required |
| GET | `/api/warehouses/{id}` | Get warehouse by ID | Required |
| GET | `/api/warehouses` | Get warehouses list | Required |
| PUT | `/api/warehouses/{id}` | Update warehouse | Required |
| DELETE | `/api/warehouses/{id}` | Delete warehouse | Required |
| POST | `/api/warehouses/{id}/set-default` | Set as default warehouse | Required |

## Frontend Components

### 1. Warehouse Service
**File**: `khidmah_inventory.client/src/app/core/services/warehouse-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class WarehouseApiService {
  private apiUrl = '/api/warehouses';

  constructor(private http: HttpClient) {}

  getWarehouses(isActive?: boolean): Observable<WarehouseDto[]> {
    const params = isActive !== undefined ? { isActive: isActive.toString() } : {};
    return this.http.get<WarehouseDto[]>(this.apiUrl, { params });
  }

  getWarehouse(id: string): Observable<WarehouseDto> {
    return this.http.get<WarehouseDto>(`${this.apiUrl}/${id}`);
  }

  createWarehouse(warehouse: CreateWarehouseDto): Observable<WarehouseDto> {
    return this.http.post<WarehouseDto>(this.apiUrl, warehouse);
  }

  updateWarehouse(id: string, warehouse: UpdateWarehouseDto): Observable<WarehouseDto> {
    return this.http.put<WarehouseDto>(`${this.apiUrl}/${id}`, warehouse);
  }

  deleteWarehouse(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  setDefaultWarehouse(id: string): Observable<WarehouseDto> {
    return this.http.post<WarehouseDto>(`${this.apiUrl}/${id}/set-default`, {});
  }
}
```

### 2. Warehouse List Component
- Display warehouses in table
- Show default warehouse indicator
- Filter by active status
- Actions: View, Edit, Delete, Set Default

### 3. Warehouse Form Component
- Create/Edit warehouse form
- Address fields
- Default warehouse checkbox
- Validation

## Workflow

### Create Warehouse Flow
```
User fills form → Validate → Submit → API → Command Handler
→ Check for default warehouse → Create entity → Save
→ Return DTO → Update list
```

### Set Default Warehouse Flow
```
User clicks "Set as Default" → API → Command Handler
→ Unset current default → Set new default → Save
→ Return updated DTO → Update UI
```

## Testing Checklist

- [ ] Create warehouse
- [ ] Create warehouse with duplicate name (should fail)
- [ ] Set default warehouse
- [ ] Only one default warehouse at a time
- [ ] Update warehouse
- [ ] Delete warehouse with stock (should fail)
- [ ] Delete warehouse without stock
- [ ] Get warehouses list
- [ ] Filter by active status
- [ ] Multi-tenancy isolation
- [ ] Frontend: Warehouse list displays correctly
- [ ] Frontend: Default warehouse indicator works

