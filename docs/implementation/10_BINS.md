# Bins Module Implementation

## Feature Overview

The Bins module manages bin locations within warehouse zones. Bins are the smallest storage location unit (e.g., "A-1-1", "B-2-3").

## Business Requirements

1. **Bin Management**
   - Create, read, update, and delete bins
   - Bin belongs to a warehouse zone
   - Bin name and code
   - Display order for sorting
   - Active/Inactive status

2. **Bin Attributes**
   - Name (required)
   - Code (optional)
   - Description
   - DisplayOrder
   - IsActive

## Domain Model

### Entity: Bin
- **Location**: `Khidmah_Inventory.Domain/Entities/Bin.cs`
- **Relationships**:
  - Belongs to: WarehouseZone

### Key Properties
- `WarehouseZoneId`, `Name`, `Code`
- `Description`, `DisplayOrder`
- `IsActive`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Bin Command
**File**: `Khidmah_Inventory.Application/Features/Bins/Commands/CreateBin/CreateBinCommand.cs`

```csharp
public class CreateBinCommand : IRequest<Result<BinDto>>
{
    public Guid WarehouseZoneId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; } = 0;
}
```

**Handler**: 
- Validate warehouse zone exists and belongs to company
- Validate name is unique within zone
- Validate code is unique within zone if provided
- Create Bin entity
- Save to database

#### Update Bin Command
- Similar validation
- Update entity

#### Activate/Deactivate Bin Commands
- Toggle IsActive status

#### Delete Bin Command
- Soft delete (set IsDeleted flag)

### 2. Application Layer - Queries

#### Get Bin Query
Load bin with zone and warehouse information.

#### Get Bins List Query
**File**: `Khidmah_Inventory.Application/Features/Bins/Queries/GetBinsList/GetBinsListQuery.cs`

```csharp
public class GetBinsListQuery : IRequest<Result<List<BinDto>>>
{
    public Guid? WarehouseZoneId { get; set; }
    public Guid? WarehouseId { get; set; }
    public bool? IsActive { get; set; }
}
```

**Handler**:
- Filter by CompanyId
- Filter by WarehouseZoneId if provided
- Filter by WarehouseId if provided
- Filter by IsActive if provided
- Order by DisplayOrder
- Return list

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/Bins/BinDto.cs`

```csharp
public class BinDto
{
    public Guid Id { get; set; }
    public Guid WarehouseZoneId { get; set; }
    public string ZoneName { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? Description { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/BinsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class BinsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateBinCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetBinQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetBinsListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateBinCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteBinCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result);
    }
    
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateBinCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateBinCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/bins` | Create bin | Required |
| GET | `/api/bins/{id}` | Get bin by ID | Required |
| GET | `/api/bins` | Get bins list | Required |
| PUT | `/api/bins/{id}` | Update bin | Required |
| DELETE | `/api/bins/{id}` | Delete bin | Required |
| POST | `/api/bins/{id}/activate` | Activate bin | Required |
| POST | `/api/bins/{id}/deactivate` | Deactivate bin | Required |

## Frontend Components

### 1. Bin Service
Similar to other services.

### 2. Bin List Component
- Display bins in table
- Filter by warehouse, zone, active status
- Show full location path (Warehouse > Zone > Bin)
- Actions: View, Edit, Delete, Activate/Deactivate

### 3. Bin Form Component
- Create/Edit bin form
- Zone selection
- Display order input
- Validation

## Testing Checklist

- [ ] Create bin
- [ ] Create bin with duplicate name in same zone (should fail)
- [ ] Update bin
- [ ] Delete bin
- [ ] Activate/Deactivate bin
- [ ] Get bins by zone
- [ ] Get bins by warehouse
- [ ] Filter by active status
- [ ] Multi-tenancy isolation
- [ ] Frontend: Bin form works correctly

