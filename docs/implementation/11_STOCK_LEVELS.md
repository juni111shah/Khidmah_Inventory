# Stock Levels Module Implementation

## Feature Overview

The Stock Levels module tracks current inventory quantities for each product in each warehouse. It provides real-time stock information and supports reserved quantities for pending orders.

## Business Requirements

1. **Stock Tracking**
   - Track quantity per product per warehouse
   - Track reserved quantity (for pending orders)
   - Calculate available quantity (quantity - reserved)
   - Track average cost
   - Last updated timestamp

2. **Stock Operations**
   - Adjust stock quantity
   - Reserve quantity for orders
   - Release reserved quantity
   - Set stock quantity (for initial setup or corrections)

3. **Stock Attributes**
   - ProductId, WarehouseId
   - Quantity (current stock)
   - ReservedQuantity (reserved for orders)
   - AvailableQuantity (calculated: Quantity - ReservedQuantity)
   - AverageCost
   - LastUpdated

## Domain Model

### Entity: StockLevel
- **Location**: `Khidmah_Inventory.Domain/Entities/StockLevel.cs`
- **Relationships**:
  - Belongs to: Product, Warehouse

### Key Properties
- `ProductId`, `WarehouseId`
- `Quantity`, `ReservedQuantity`
- `AvailableQuantity` (calculated property)
- `AverageCost`
- `LastUpdated`

## Implementation Steps

### 1. Application Layer - Commands

#### Adjust Stock Command
**File**: `Khidmah_Inventory.Application/Features/StockLevels/Commands/AdjustStock/AdjustStockCommand.cs`

```csharp
public class AdjustStockCommand : IRequest<Result<StockLevelDto>>
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; } // Positive for increase, negative for decrease
    public decimal? AverageCost { get; set; }
    public string? Reason { get; set; }
}
```

**Handler**: 
- Load or create StockLevel
- Adjust quantity
- Update average cost if provided
- Create StockTransaction record
- Save changes

#### Reserve Stock Command
**File**: `Khidmah_Inventory.Application/Features/StockLevels/Commands/ReserveStock/ReserveStockCommand.cs`

```csharp
public class ReserveStockCommand : IRequest<Result>
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public decimal Quantity { get; set; }
    public string ReferenceType { get; set; } = string.Empty; // "SalesOrder", etc.
    public Guid? ReferenceId { get; set; }
}
```

**Handler**:
- Load StockLevel
- Verify available quantity >= quantity to reserve
- Reserve quantity
- Save changes

#### Release Reserved Stock Command
- Release reserved quantity
- Used when order is cancelled or fulfilled

### 2. Application Layer - Queries

#### Get Stock Level Query
**File**: `Khidmah_Inventory.Application/Features/StockLevels/Queries/GetStockLevel/GetStockLevelQuery.cs`

```csharp
public class GetStockLevelQuery : IRequest<Result<StockLevelDto>>
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
}
```

**Handler**:
- Load StockLevel by ProductId and WarehouseId
- Filter by CompanyId
- Return StockLevelDto

#### Get Stock Levels by Product Query
Get all stock levels for a product across warehouses.

#### Get Stock Levels by Warehouse Query
Get all stock levels for a warehouse across products.

#### Get Low Stock Products Query
**File**: `Khidmah_Inventory.Application/Features/StockLevels/Queries/GetLowStockProducts/GetLowStockProductsQuery.cs`

```csharp
public class GetLowStockProductsQuery : IRequest<Result<List<StockLevelDto>>>
{
    public Guid? WarehouseId { get; set; }
}
```

**Handler**:
- Find products where Quantity <= ReorderPoint
- Filter by WarehouseId if provided
- Return list

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/StockLevels/StockLevelDto.cs`

```csharp
public class StockLevelDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal ReservedQuantity { get; set; }
    public decimal AvailableQuantity { get; set; }
    public decimal? AverageCost { get; set; }
    public decimal? ReorderPoint { get; set; }
    public decimal? MinStockLevel { get; set; }
    public decimal? MaxStockLevel { get; set; }
    public bool IsLowStock { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/StockLevelConfiguration.cs`

```csharp
public class StockLevelConfiguration : IEntityTypeConfiguration<StockLevel>
{
    public void Configure(EntityTypeBuilder<StockLevel> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Quantity)
            .HasPrecision(18, 4)
            .HasDefaultValue(0);
            
        builder.Property(s => s.ReservedQuantity)
            .HasPrecision(18, 4)
            .HasDefaultValue(0);
            
        builder.Property(s => s.AverageCost)
            .HasPrecision(18, 2);
            
        // Unique index: one stock level per product per warehouse
        builder.HasIndex(s => new { s.CompanyId, s.ProductId, s.WarehouseId })
            .IsUnique();
            
        // Indexes for queries
        builder.HasIndex(s => s.ProductId);
        builder.HasIndex(s => s.WarehouseId);
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/StockLevelsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockLevelsController : BaseApiController
{
    [HttpGet]
    public async Task<IActionResult> Get([FromQuery] GetStockLevelQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var query = new GetStockLevelsByProductQuery { ProductId = productId };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("warehouse/{warehouseId}")]
    public async Task<IActionResult> GetByWarehouse(Guid warehouseId)
    {
        var query = new GetStockLevelsByWarehouseQuery { WarehouseId = warehouseId };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("low-stock")]
    public async Task<IActionResult> GetLowStock([FromQuery] GetLowStockProductsQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("adjust")]
    public async Task<IActionResult> AdjustStock([FromBody] AdjustStockCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("reserve")]
    public async Task<IActionResult> ReserveStock([FromBody] ReserveStockCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok() : BadRequest(result);
    }
    
    [HttpPost("release")]
    public async Task<IActionResult> ReleaseStock([FromBody] ReleaseReservedStockCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok() : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/stocklevels` | Get stock level (by product & warehouse) | Required |
| GET | `/api/stocklevels/product/{productId}` | Get stock levels by product | Required |
| GET | `/api/stocklevels/warehouse/{warehouseId}` | Get stock levels by warehouse | Required |
| GET | `/api/stocklevels/low-stock` | Get low stock products | Required |
| POST | `/api/stocklevels/adjust` | Adjust stock quantity | Required |
| POST | `/api/stocklevels/reserve` | Reserve stock | Required |
| POST | `/api/stocklevels/release` | Release reserved stock | Required |

## Frontend Components

### 1. Stock Level Service
**File**: `khidmah_inventory.client/src/app/core/services/stock-level-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class StockLevelApiService {
  private apiUrl = '/api/stocklevels';

  constructor(private http: HttpClient) {}

  getStockLevel(productId: string, warehouseId: string): Observable<StockLevelDto> {
    return this.http.get<StockLevelDto>(this.apiUrl, {
      params: { productId, warehouseId }
    });
  }

  getStockLevelsByProduct(productId: string): Observable<StockLevelDto[]> {
    return this.http.get<StockLevelDto[]>(`${this.apiUrl}/product/${productId}`);
  }

  getStockLevelsByWarehouse(warehouseId: string): Observable<StockLevelDto[]> {
    return this.http.get<StockLevelDto[]>(`${this.apiUrl}/warehouse/${warehouseId}`);
  }

  getLowStockProducts(warehouseId?: string): Observable<StockLevelDto[]> {
    const params = warehouseId ? { warehouseId } : {};
    return this.http.get<StockLevelDto[]>(`${this.apiUrl}/low-stock`, { params });
  }

  adjustStock(command: AdjustStockCommand): Observable<StockLevelDto> {
    return this.http.post<StockLevelDto>(`${this.apiUrl}/adjust`, command);
  }

  reserveStock(command: ReserveStockCommand): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/reserve`, command);
  }

  releaseStock(command: ReleaseReservedStockCommand): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/release`, command);
  }
}
```

### 2. Stock Level List Component
- Display stock levels in table
- Filter by warehouse or product
- Show low stock indicators
- Actions: Adjust, View Details

### 3. Stock Adjustment Component
- Form to adjust stock
- Reason input
- Quantity input (positive/negative)
- Cost input

## Workflow

### Adjust Stock Flow
```
User enters adjustment → Validate → Submit → API → Command Handler
→ Load or create StockLevel → Adjust quantity → Create StockTransaction
→ Save → Return updated StockLevel → Update UI
```

### Reserve Stock Flow
```
Order created → Reserve stock for each item → API → Command Handler
→ Load StockLevel → Verify available quantity → Reserve
→ Save → Return success
```

## Testing Checklist

- [ ] Get stock level by product and warehouse
- [ ] Get stock levels by product
- [ ] Get stock levels by warehouse
- [ ] Adjust stock (increase)
- [ ] Adjust stock (decrease)
- [ ] Adjust stock below zero (should fail or allow negative)
- [ ] Reserve stock
- [ ] Reserve more than available (should fail)
- [ ] Release reserved stock
- [ ] Get low stock products
- [ ] Multi-tenancy isolation
- [ ] Frontend: Stock level display works
- [ ] Frontend: Stock adjustment form works

