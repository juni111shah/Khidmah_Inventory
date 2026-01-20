# Purchase Orders Module Implementation

## Feature Overview

The Purchase Orders module manages purchase orders from suppliers. It tracks orders from creation through receipt, including order items, pricing, discounts, taxes, and status management.

## Business Requirements

1. **Purchase Order Management**
   - Create, read, update, and delete purchase orders
   - Order number generation
   - Supplier selection
   - Order date and expected delivery date
   - Status tracking (Draft, Sent, PartiallyReceived, Completed, Cancelled)

2. **Order Items**
   - Multiple products per order
   - Quantity, unit price per item
   - Discount (percentage or amount)
   - Tax (percentage)
   - Line total calculation
   - Received quantity tracking

3. **Order Totals**
   - Subtotal (sum of line totals)
   - Total discount
   - Total tax
   - Grand total

4. **Order Workflow**
   - Draft → Sent → PartiallyReceived → Completed
   - Can be cancelled at any stage
   - Receiving updates stock levels

## Domain Model

### Entity: PurchaseOrder
- **Location**: `Khidmah_Inventory.Domain/Entities/PurchaseOrder.cs`
- **Relationships**:
  - Belongs to: Supplier
  - Has many: PurchaseOrderItems

### Entity: PurchaseOrderItem
- **Location**: `Khidmah_Inventory.Domain/Entities/PurchaseOrderItem.cs`
- **Relationships**:
  - Belongs to: PurchaseOrder, Product

### Key Properties
- `OrderNumber`, `SupplierId`, `OrderDate`
- `ExpectedDeliveryDate`, `Status`
- `SubTotal`, `TaxAmount`, `DiscountAmount`, `TotalAmount`
- `Notes`, `TermsAndConditions`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Purchase Order Command
**File**: `Khidmah_Inventory.Application/Features/PurchaseOrders/Commands/CreatePurchaseOrder/CreatePurchaseOrderCommand.cs`

```csharp
public class CreatePurchaseOrderCommand : IRequest<Result<PurchaseOrderDto>>
{
    public Guid SupplierId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public List<CreatePurchaseOrderItemDto> Items { get; set; } = new();
}

public class CreatePurchaseOrderItemDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal TaxPercent { get; set; }
    public string? Notes { get; set; }
}
```

**Handler**: 
- Generate order number (e.g., PO-YYYYMMDD-001)
- Validate supplier exists and is active
- Validate all products exist
- Create PurchaseOrder entity
- Create PurchaseOrderItem entities
- Calculate totals
- Save to database

#### Update Purchase Order Command
- Similar to create
- Only allow updates if status is Draft
- Recalculate totals

#### Update Purchase Order Status Command
**File**: `Khidmah_Inventory.Application/Features/PurchaseOrders/Commands/UpdatePurchaseOrderStatus/UpdatePurchaseOrderStatusCommand.cs`

```csharp
public class UpdatePurchaseOrderStatusCommand : IRequest<Result<PurchaseOrderDto>>
{
    public Guid Id { get; set; }
    public string Status { get; set; } = string.Empty; // Sent, PartiallyReceived, Completed, Cancelled
}
```

**Handler**:
- Load purchase order
- Validate status transition
- Update status
- If Completed, update stock levels (create StockTransaction for each item)
- Save changes

#### Receive Purchase Order Command
**File**: `Khidmah_Inventory.Application/Features/PurchaseOrders/Commands/ReceivePurchaseOrder/ReceivePurchaseOrderCommand.cs`

```csharp
public class ReceivePurchaseOrderCommand : IRequest<Result>
{
    public Guid PurchaseOrderId { get; set; }
    public Guid WarehouseId { get; set; }
    public List<ReceiveItemDto> Items { get; set; } = new();
}

public class ReceiveItemDto
{
    public Guid PurchaseOrderItemId { get; set; }
    public decimal Quantity { get; set; }
}
```

**Handler**:
- Load purchase order
- Validate warehouse exists
- For each item, update ReceivedQuantity
- Create StockTransaction (StockIn)
- Update StockLevel
- Update order status if all items received
- Save changes

### 2. Application Layer - Queries

#### Get Purchase Order Query
Load purchase order with items, supplier, and products.

#### Get Purchase Orders List Query
**File**: `Khidmah_Inventory.Application/Features/PurchaseOrders/Queries/GetPurchaseOrdersList/GetPurchaseOrdersListQuery.cs`

```csharp
public class GetPurchaseOrdersListQuery : IRequest<Result<PagedResult<PurchaseOrderDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? SupplierId { get; set; }
    public string? Status { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}
```

**Handler**:
- Filter by CompanyId
- Apply filters
- Apply pagination
- Return paged results

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/PurchaseOrders/PurchaseOrderDto.cs`

```csharp
public class PurchaseOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid SupplierId { get; set; }
    public string SupplierName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public List<PurchaseOrderItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PurchaseOrderItemDto
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public string ProductSKU { get; set; } = string.Empty;
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountPercent { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxPercent { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
    public decimal ReceivedQuantity { get; set; }
    public string? Notes { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/PurchaseOrderConfiguration.cs`

```csharp
public class PurchaseOrderConfiguration : IEntityTypeConfiguration<PurchaseOrder>
{
    public void Configure(EntityTypeBuilder<PurchaseOrder> builder)
    {
        builder.HasKey(po => po.Id);
        
        builder.Property(po => po.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(po => po.TotalAmount)
            .HasPrecision(18, 2);
            
        // Indexes
        builder.HasIndex(po => new { po.CompanyId, po.OrderNumber })
            .IsUnique();
            
        builder.HasIndex(po => po.SupplierId);
        builder.HasIndex(po => po.Status);
        builder.HasIndex(po => po.OrderDate);
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/PurchaseOrdersController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class PurchaseOrdersController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePurchaseOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetPurchaseOrderQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetPurchaseOrdersListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdatePurchaseOrderCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/status")]
    public async Task<IActionResult> UpdateStatus(Guid id, [FromBody] UpdatePurchaseOrderStatusCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/receive")]
    public async Task<IActionResult> Receive(Guid id, [FromBody] ReceivePurchaseOrderCommand command)
    {
        command.PurchaseOrderId = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok() : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/purchaseorders` | Create purchase order | Required |
| GET | `/api/purchaseorders/{id}` | Get purchase order by ID | Required |
| GET | `/api/purchaseorders` | Get purchase orders list (paginated) | Required |
| PUT | `/api/purchaseorders/{id}` | Update purchase order | Required |
| POST | `/api/purchaseorders/{id}/status` | Update order status | Required |
| POST | `/api/purchaseorders/{id}/receive` | Receive order items | Required |

## Frontend Components

### 1. Purchase Order Service
**File**: `khidmah_inventory.client/src/app/core/services/purchase-order-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class PurchaseOrderApiService {
  private apiUrl = '/api/purchaseorders';

  constructor(private http: HttpClient) {}

  getPurchaseOrders(params?: any): Observable<PagedResult<PurchaseOrderDto>> {
    return this.http.get<PagedResult<PurchaseOrderDto>>(this.apiUrl, { params });
  }

  getPurchaseOrder(id: string): Observable<PurchaseOrderDto> {
    return this.http.get<PurchaseOrderDto>(`${this.apiUrl}/${id}`);
  }

  createPurchaseOrder(order: CreatePurchaseOrderDto): Observable<PurchaseOrderDto> {
    return this.http.post<PurchaseOrderDto>(this.apiUrl, order);
  }

  updatePurchaseOrder(id: string, order: UpdatePurchaseOrderDto): Observable<PurchaseOrderDto> {
    return this.http.put<PurchaseOrderDto>(`${this.apiUrl}/${id}`, order);
  }

  updateStatus(id: string, status: string): Observable<PurchaseOrderDto> {
    return this.http.post<PurchaseOrderDto>(`${this.apiUrl}/${id}/status`, { status });
  }

  receiveOrder(id: string, receiveData: ReceivePurchaseOrderDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/receive`, receiveData);
  }
}
```

### 2. Purchase Order List Component
- Display orders in table
- Filter by supplier, status, date range
- Search functionality
- Actions: View, Edit, Receive, Cancel

### 3. Purchase Order Form Component
- Create/Edit purchase order
- Supplier selection
- Add/remove items
- Product selection with autocomplete
- Quantity, price, discount, tax inputs
- Auto-calculate totals
- Save as draft or send

### 4. Receive Order Component
- Select warehouse
- Enter received quantities per item
- Submit receipt
- Update stock levels

## Workflow

### Create Purchase Order Flow
```
User selects supplier → Add items → Enter quantities and prices
→ Calculate totals → Save as Draft → Generate order number
→ API → Command Handler → Create entities → Save → Return DTO
```

### Receive Order Flow
```
User selects order → Select warehouse → Enter received quantities
→ Submit → API → Command Handler → Update received quantities
→ Create StockTransactions → Update StockLevels → Update order status
→ Save → Return success
```

## Testing Checklist

- [ ] Create purchase order
- [ ] Create purchase order with items
- [ ] Calculate totals correctly
- [ ] Update purchase order (draft only)
- [ ] Update order status
- [ ] Receive order items
- [ ] Receive partial quantities
- [ ] Stock levels update on receipt
- [ ] Get purchase orders list with filters
- [ ] Multi-tenancy isolation
- [ ] Frontend: Order form works correctly
- [ ] Frontend: Totals calculate correctly
- [ ] Frontend: Receive form works

