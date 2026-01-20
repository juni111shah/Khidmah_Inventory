# Warehouse Zones Module Implementation

## Feature Overview

The Warehouse Zones module manages zones within warehouses. Zones are organizational units within a warehouse (e.g., "A Zone", "B Zone", "Cold Storage").

## Business Requirements

1. **Zone Management**
   - Create, read, update, and delete zones
   - Zone belongs to a warehouse
   - Zone name and code
   - Display order for sorting

2. **Zone Attributes**
   - Name (required)
   - Code (optional)
   - Description
   - DisplayOrder

## Domain Model

### Entity: WarehouseZone
- **Location**: `Khidmah_Inventory.Domain/Entities/WarehouseZone.cs`
- **Relationships**:
  - Belongs to: Warehouse
  - Has many: Bins

### Key Properties
- `WarehouseId`, `Name`, `Code`
- `Description`, `DisplayOrder`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Warehouse Zone Command
**File**: `Khidmah_Inventory.Application/Features/WarehouseZones/Commands/CreateWarehouseZone/CreateWarehouseZoneCommand.cs`

```csharp
public class CreateWarehouseZoneCommand : IRequest<Result<WarehouseZoneDto>>
{
    public Guid WarehouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; } = 0;
}
```

**Handler**: 
- Validate warehouse exists and belongs to company
- Validate name is unique within warehouse
- Validate code is unique within warehouse if provided
- Create WarehouseZone entity
- Save to database

#### Update Warehouse Zone Command
- Similar validation
- Update entity

#### Delete Warehouse Zone Command
- Check if zone has bins
- Prevent deletion if has bins

### 2. Application Layer - Queries

#### Get Warehouse Zone Query
Load zone with warehouse and bin count.

#### Get Warehouse Zones List Query
**File**: `Khidmah_Inventory.Application/Features/WarehouseZones/Queries/GetWarehouseZonesList/GetWarehouseZonesListQuery.cs`

```csharp
public class GetWarehouseZonesListQuery : IRequest<Result<List<WarehouseZoneDto>>>
{
    public Guid WarehouseId { get; set; }
}
```

**Handler**:
- Filter by CompanyId and WarehouseId
- Order by DisplayOrder
- Return list

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/WarehouseZones/WarehouseZoneDto.cs`

```csharp
public class WarehouseZoneDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public int BinCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/WarehouseZonesController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WarehouseZonesController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateWarehouseZoneCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetWarehouseZoneQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetWarehouseZonesListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateWarehouseZoneCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteWarehouseZoneCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/warehousezones` | Create zone | Required |
| GET | `/api/warehousezones/{id}` | Get zone by ID | Required |
| GET | `/api/warehousezones` | Get zones list (by warehouse) | Required |
| PUT | `/api/warehousezones/{id}` | Update zone | Required |
| DELETE | `/api/warehousezones/{id}` | Delete zone | Required |

## Frontend Components

### 1. Warehouse Zone Service
Similar to other services.

### 2. Warehouse Zone List Component
- Display zones in table
- Filter by warehouse
- Show bin count
- Actions: View, Edit, Delete

### 3. Warehouse Zone Form Component
- Create/Edit zone form
- Warehouse selection
- Display order input
- Validation

## Testing Checklist

- [ ] Create zone
- [ ] Create zone with duplicate name in same warehouse (should fail)
- [ ] Update zone
- [ ] Delete zone with bins (should fail)
- [ ] Delete zone without bins
- [ ] Get zones by warehouse
- [ ] Multi-tenancy isolation
- [ ] Frontend: Zone form works correctly

