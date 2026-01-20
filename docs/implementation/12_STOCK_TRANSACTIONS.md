# Stock Transactions Module Implementation

## Feature Overview

The Stock Transactions module tracks all stock movements (in, out, adjustments, transfers). It provides a complete audit trail of inventory changes.

## Business Requirements

1. **Transaction Tracking**
   - Record all stock movements
   - Transaction types: StockIn, StockOut, Adjustment, Transfer
   - Reference to source document (PO, SO, etc.)
   - Batch and expiry tracking
   - Transaction date and balance after

2. **Transaction Attributes**
   - ProductId, WarehouseId
   - TransactionType
   - Quantity (positive for in, negative for out)
   - UnitCost, TotalCost
   - ReferenceType, ReferenceId, ReferenceNumber
   - BatchNumber, ExpiryDate
   - BalanceAfter (stock level after transaction)

3. **Transaction History**
   - View transaction history by product
   - View transaction history by warehouse
   - Filter by transaction type
   - Filter by date range
   - Filter by reference

## Domain Model

### Entity: StockTransaction
- **Location**: `Khidmah_Inventory.Domain/Entities/StockTransaction.cs`
- **Relationships**:
  - Belongs to: Product, Warehouse

### Key Properties
- `ProductId`, `WarehouseId`
- `TransactionType` (StockIn, StockOut, Adjustment, Transfer)
- `Quantity`, `UnitCost`, `TotalCost`
- `ReferenceType`, `ReferenceId`, `ReferenceNumber`
- `BatchNumber`, `ExpiryDate`
- `BalanceAfter`, `TransactionDate`

## Implementation Steps

### 1. Application Layer - Commands

Stock transactions are typically created automatically by other modules (Purchase Orders, Sales Orders, Stock Adjustments). However, manual transactions can be created:

#### Create Stock Transaction Command
**File**: `Khidmah_Inventory.Application/Features/StockTransactions/Commands/CreateStockTransaction/CreateStockTransactionCommand.cs`

```csharp
public class CreateStockTransactionCommand : IRequest<Result<StockTransactionDto>>
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // StockIn, StockOut, Adjustment
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
}
```

**Handler**: 
- Validate product and warehouse exist
- Create StockTransaction entity
- Update StockLevel
- Calculate BalanceAfter
- Save changes

### 2. Application Layer - Queries

#### Get Stock Transaction Query
Load transaction with product and warehouse details.

#### Get Stock Transactions List Query
**File**: `Khidmah_Inventory.Application/Features/StockTransactions/Queries/GetStockTransactionsList/GetStockTransactionsListQuery.cs`

```csharp
public class GetStockTransactionsListQuery : IRequest<Result<PagedResult<StockTransactionDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public Guid? ProductId { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? TransactionType { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
```

**Handler**:
- Filter by CompanyId
- Apply all filters
- Order by TransactionDate descending
- Apply pagination
- Return paged results

#### Get Product Transaction History Query
Get all transactions for a specific product.

#### Get Warehouse Transaction History Query
Get all transactions for a specific warehouse.

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/StockTransactions/StockTransactionDto.cs`

```csharp
public class StockTransactionDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public string TransactionType { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public decimal? TotalCost { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public DateTime TransactionDate { get; set; }
    public decimal BalanceAfter { get; set; }
    public string CreatedByUserName { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/StockTransactionConfiguration.cs`

```csharp
public class StockTransactionConfiguration : IEntityTypeConfiguration<StockTransaction>
{
    public void Configure(EntityTypeBuilder<StockTransaction> builder)
    {
        builder.HasKey(st => st.Id);
        
        builder.Property(st => st.Quantity)
            .HasPrecision(18, 4);
            
        builder.Property(st => st.UnitCost)
            .HasPrecision(18, 2);
            
        builder.Property(st => st.BalanceAfter)
            .HasPrecision(18, 4);
            
        // Indexes
        builder.HasIndex(st => st.ProductId);
        builder.HasIndex(st => st.WarehouseId);
        builder.HasIndex(st => st.TransactionType);
        builder.HasIndex(st => st.TransactionDate);
        builder.HasIndex(st => new { st.ReferenceType, st.ReferenceId });
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/StockTransactionsController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class StockTransactionsController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateStockTransactionCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetStockTransactionQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetStockTransactionsListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("product/{productId}")]
    public async Task<IActionResult> GetByProduct(Guid productId)
    {
        var query = new GetProductTransactionHistoryQuery { ProductId = productId };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("warehouse/{warehouseId}")]
    public async Task<IActionResult> GetByWarehouse(Guid warehouseId)
    {
        var query = new GetWarehouseTransactionHistoryQuery { WarehouseId = warehouseId };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/stocktransactions` | Create transaction (manual) | Required |
| GET | `/api/stocktransactions/{id}` | Get transaction by ID | Required |
| GET | `/api/stocktransactions` | Get transactions list (paginated) | Required |
| GET | `/api/stocktransactions/product/{productId}` | Get product history | Required |
| GET | `/api/stocktransactions/warehouse/{warehouseId}` | Get warehouse history | Required |

## Frontend Components

### 1. Stock Transaction Service
Similar to other services.

### 2. Stock Transaction List Component
- Display transactions in table
- Filter by product, warehouse, type, date range
- Show transaction details
- Link to reference document

### 3. Stock Transaction Form Component
- Manual transaction entry
- Product and warehouse selection
- Transaction type selection
- Quantity and cost inputs
- Reference information
- Batch and expiry (if applicable)

## Workflow

### Automatic Transaction Creation
```
Purchase Order received → Create StockTransaction (StockIn)
→ Update StockLevel → Calculate BalanceAfter → Save
```

### Manual Transaction Flow
```
User enters transaction details → Validate → Submit
→ API → Command Handler → Create transaction
→ Update StockLevel → Save → Return DTO
```

## Testing Checklist

- [ ] Create manual transaction
- [ ] Transaction automatically created on PO receipt
- [ ] Transaction automatically created on SO delivery
- [ ] Get transactions by product
- [ ] Get transactions by warehouse
- [ ] Filter by transaction type
- [ ] Filter by date range
- [ ] Filter by reference
- [ ] BalanceAfter calculated correctly
- [ ] Multi-tenancy isolation
- [ ] Frontend: Transaction list displays correctly
- [ ] Frontend: Filters work correctly

