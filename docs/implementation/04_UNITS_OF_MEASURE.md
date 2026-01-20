# Units of Measure Module Implementation

## Feature Overview

The Units of Measure (UOM) module manages measurement units used for products. Supports base units and conversion factors for unit conversions (e.g., 1 box = 12 pieces).

## Business Requirements

1. **Unit Management**
   - Create, read, update, and delete units
   - Support base units and derived units
   - Conversion factor support
   - Unit code (e.g., "PC", "BOX", "KG")

2. **Unit Conversion**
   - Base units (e.g., "Piece")
   - Derived units with conversion factors (e.g., "Box" = 12 pieces)
   - Prevent circular conversions

3. **Unit Attributes**
   - Name (e.g., "Piece", "Box")
   - Code (e.g., "PC", "BOX")
   - Description
   - IsBaseUnit flag
   - ConversionFactor (for derived units)
   - BaseUnitId (for derived units)

## Domain Model

### Entity: UnitOfMeasure
- **Location**: `Khidmah_Inventory.Domain/Entities/UnitOfMeasure.cs`
- **Relationships**:
  - Self-referencing: BaseUnit → Derived units
  - Has many: Products

### Key Properties
- `Name`, `Code`, `Description`
- `IsBaseUnit`
- `ConversionFactor`, `BaseUnitId`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Unit Command
**File**: `Khidmah_Inventory.Application/Features/UnitsOfMeasure/Commands/CreateUnitOfMeasure/CreateUnitOfMeasureCommand.cs`

```csharp
public class CreateUnitOfMeasureCommand : IRequest<Result<UnitOfMeasureDto>>
{
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsBaseUnit { get; set; } = false;
    public decimal? ConversionFactor { get; set; }
    public Guid? BaseUnitId { get; set; }
}
```

**Handler**: 
- Validate code is unique within company
- If IsBaseUnit, ensure ConversionFactor and BaseUnitId are null
- If not base unit, validate BaseUnitId exists and ConversionFactor > 0
- Create UnitOfMeasure entity
- Save to database

**Validator**:
```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(100);
RuleFor(x => x.Code).NotEmpty().MaximumLength(20);
RuleFor(x => x.ConversionFactor)
    .GreaterThan(0)
    .When(x => !x.IsBaseUnit && x.BaseUnitId.HasValue);
RuleFor(x => x.BaseUnitId)
    .NotEmpty()
    .When(x => !x.IsBaseUnit);
```

#### Update Unit Command
- Similar validation
- Prevent changing base unit if used by derived units

#### Delete Unit Command
- Check if unit is used by products
- Check if unit is base unit for other units
- Prevent deletion if in use

### 2. Application Layer - Queries

#### Get Unit Query
Load unit with base unit information.

#### Get Units List Query
**File**: `Khidmah_Inventory.Application/Features/UnitsOfMeasure/Queries/GetUnitsList/GetUnitsListQuery.cs`

```csharp
public class GetUnitsListQuery : IRequest<Result<List<UnitOfMeasureDto>>>
{
    public bool? IsBaseUnit { get; set; }
}
```

**Handler**:
- Filter by CompanyId
- Filter by IsBaseUnit if provided
- Order by name
- Return list

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/UnitsOfMeasure/UnitOfMeasureDto.cs`

```csharp
public class UnitOfMeasureDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Code { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsBaseUnit { get; set; }
    public decimal? ConversionFactor { get; set; }
    public Guid? BaseUnitId { get; set; }
    public string? BaseUnitName { get; set; }
    public int ProductCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/UnitOfMeasureConfiguration.cs`

```csharp
public class UnitOfMeasureConfiguration : IEntityTypeConfiguration<UnitOfMeasure>
{
    public void Configure(EntityTypeBuilder<UnitOfMeasure> builder)
    {
        builder.HasKey(u => u.Id);
        
        builder.Property(u => u.Name)
            .IsRequired()
            .HasMaxLength(100);
            
        builder.Property(u => u.Code)
            .IsRequired()
            .HasMaxLength(20);
            
        builder.Property(u => u.ConversionFactor)
            .HasPrecision(18, 6);
            
        // Indexes
        builder.HasIndex(u => new { u.CompanyId, u.Code })
            .IsUnique();
            
        // Self-referencing relationship
        builder.HasOne(u => u.BaseUnit)
            .WithMany()
            .HasForeignKey(u => u.BaseUnitId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/UnitsOfMeasureController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UnitsOfMeasureController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateUnitOfMeasureCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetUnitOfMeasureQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetUnitsListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateUnitOfMeasureCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteUnitOfMeasureCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/unitsofmeasure` | Create unit | Required |
| GET | `/api/unitsofmeasure/{id}` | Get unit by ID | Required |
| GET | `/api/unitsofmeasure` | Get units list | Required |
| PUT | `/api/unitsofmeasure/{id}` | Update unit | Required |
| DELETE | `/api/unitsofmeasure/{id}` | Delete unit | Required |

## Frontend Components

### 1. Unit Service
**File**: `khidmah_inventory.client/src/app/core/services/unit-of-measure-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class UnitOfMeasureApiService {
  private apiUrl = '/api/unitsofmeasure';

  constructor(private http: HttpClient) {}

  getUnits(isBaseUnit?: boolean): Observable<UnitOfMeasureDto[]> {
    const params = isBaseUnit !== undefined ? { isBaseUnit: isBaseUnit.toString() } : {};
    return this.http.get<UnitOfMeasureDto[]>(this.apiUrl, { params });
  }

  getUnit(id: string): Observable<UnitOfMeasureDto> {
    return this.http.get<UnitOfMeasureDto>(`${this.apiUrl}/${id}`);
  }

  createUnit(unit: CreateUnitOfMeasureDto): Observable<UnitOfMeasureDto> {
    return this.http.post<UnitOfMeasureDto>(this.apiUrl, unit);
  }

  updateUnit(id: string, unit: UpdateUnitOfMeasureDto): Observable<UnitOfMeasureDto> {
    return this.http.put<UnitOfMeasureDto>(`${this.apiUrl}/${id}`, unit);
  }

  deleteUnit(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }
}
```

### 2. Unit List Component
- Display units in table
- Filter by base/derived units
- Show conversion relationships
- Actions: View, Edit, Delete

### 3. Unit Form Component
- Create/Edit unit form
- Base unit toggle
- Base unit dropdown (when not base unit)
- Conversion factor input
- Validation

## Workflow

### Create Base Unit Flow
```
User selects "Base Unit" → Enter name and code → Submit
→ Validate code uniqueness → Create entity → Save
```

### Create Derived Unit Flow
```
User deselects "Base Unit" → Select base unit → Enter conversion factor
→ Validate → Create entity → Save
```

## Testing Checklist

- [ ] Create base unit
- [ ] Create derived unit with valid conversion factor
- [ ] Prevent creating derived unit without base unit
- [ ] Prevent duplicate code within company
- [ ] Update unit
- [ ] Delete unit used by products (should fail)
- [ ] Delete unit used as base unit (should fail)
- [ ] Get units list
- [ ] Filter by base/derived units
- [ ] Multi-tenancy isolation
- [ ] Frontend: Unit form validation works
- [ ] Frontend: Base unit selection works correctly

