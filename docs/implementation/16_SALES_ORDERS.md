# Sales Orders Module Implementation

## Feature Overview

The Sales Orders module manages sales orders to customers. It tracks orders from creation through delivery, including order items, pricing, discounts, taxes, and status management. Sales orders reduce stock levels when delivered.

## Business Requirements

1. **Sales Order Management**
   - Create, read, update, and delete sales orders
   - Order number generation
   - Customer selection
   - Order date and expected delivery date
   - Status tracking (Draft, Confirmed, PartiallyDelivered, Delivered, Invoiced, Cancelled)

2. **Order Items**
   - Multiple products per order
   - Quantity, unit price per item
   - Discount (percentage or amount)
   - Tax (percentage)
   - Line total calculation
   - Delivered quantity tracking

3. **Order Totals**
   - Subtotal (sum of line totals)
   - Total discount
   - Total tax
   - Grand total

4. **Stock Management**
   - Reserve stock when order is confirmed
   - Reduce stock when order is delivered
   - Release reserved stock if order is cancelled

5. **Order Workflow**
   - Draft → Confirmed → PartiallyDelivered → Delivered → Invoiced
   - Can be cancelled at any stage
   - Delivery updates stock levels

## Domain Model

### Entity: SalesOrder
- **Location**: `Khidmah_Inventory.Domain/Entities/SalesOrder.cs`
- **Relationships**:
  - Belongs to: Customer
  - Has many: SalesOrderItems

### Entity: SalesOrderItem
- **Location**: `Khidmah_Inventory.Domain/Entities/SalesOrderItem.cs`
- **Relationships**:
  - Belongs to: SalesOrder, Product

### Key Properties
- `OrderNumber`, `CustomerId`, `OrderDate`
- `ExpectedDeliveryDate`, `Status`
- `SubTotal`, `TaxAmount`, `DiscountAmount`, `TotalAmount`
- `Notes`, `TermsAndConditions`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Sales Order Command
**File**: `Khidmah_Inventory.Application/Features/SalesOrders/Commands/CreateSalesOrder/CreateSalesOrderCommand.cs`

```csharp
public class CreateSalesOrderCommand : IRequest<Result<SalesOrderDto>>
{
    public Guid CustomerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.UtcNow;
    public DateTime? ExpectedDeliveryDate { get; set; }
    public Guid WarehouseId { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public List<CreateSalesOrderItemDto> Items { get; set; } = new();
}

public class CreateSalesOrderItemDto
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
- Generate order number (e.g., SO-YYYYMMDD-001)
- Validate customer exists and is active
- Validate all products exist
- Validate stock availability (if confirming immediately)
- Create SalesOrder entity
- Create SalesOrderItem entities
- Calculate totals
- Save to database

#### Update Sales Order Command
- Similar to create
- Only allow updates if status is Draft
- Recalculate totals

#### Confirm Sales Order Command
**File**: `Khidmah_Inventory.Application/Features/SalesOrders/Commands/ConfirmSalesOrder/ConfirmSalesOrderCommand.cs`

```csharp
public class ConfirmSalesOrderCommand : IRequest<Result<SalesOrderDto>>
{
    public Guid Id { get; set; }
}
```

**Handler**:
- Load sales order
- Validate status is Draft
- For each item, check stock availability
- Reserve stock for each item
- Update status to Confirmed
- Save changes

#### Deliver Sales Order Command
**File**: `Khidmah_Inventory.Application/Features/SalesOrders/Commands/DeliverSalesOrder/DeliverSalesOrderCommand.cs`

```csharp
public class DeliverSalesOrderCommand : IRequest<Result>
{
    public Guid SalesOrderId { get; set; }
    public List<DeliverItemDto> Items { get; set; } = new();
}

public class DeliverItemDto
{
    public Guid SalesOrderItemId { get; set; }
    public decimal Quantity { get; set; }
}
```

**Handler**:
- Load sales order
- Validate status allows delivery
- For each item, update DeliveredQuantity
- Create StockTransaction (StockOut)
- Update StockLevel (reduce quantity, release reserved)
- Update order status if all items delivered
- Save changes

#### Cancel Sales Order Command
- Release reserved stock
- Update status to Cancelled

### 2. Application Layer - Queries

#### Get Sales Order Query
Load sales order with items, customer, and products.

#### Get Sales Orders List Query
**File**: `Khidmah_Inventory.Application/Features/SalesOrders/Queries/GetSalesOrdersList/GetSalesOrdersListQuery.cs`

```csharp
public class GetSalesOrdersListQuery : IRequest<Result<PagedResult<SalesOrderDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public Guid? CustomerId { get; set; }
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

**File**: `Khidmah_Inventory.Application/Features/SalesOrders/SalesOrderDto.cs`

```csharp
public class SalesOrderDto
{
    public Guid Id { get; set; }
    public string OrderNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime OrderDate { get; set; }
    public DateTime? ExpectedDeliveryDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string? Notes { get; set; }
    public string? TermsAndConditions { get; set; }
    public List<SalesOrderItemDto> Items { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class SalesOrderItemDto
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
    public decimal DeliveredQuantity { get; set; }
    public string? Notes { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/SalesOrderConfiguration.cs`

```csharp
public class SalesOrderConfiguration : IEntityTypeConfiguration<SalesOrder>
{
    public void Configure(EntityTypeBuilder<SalesOrder> builder)
    {
        builder.HasKey(so => so.Id);
        
        builder.Property(so => so.OrderNumber)
            .IsRequired()
            .HasMaxLength(50);
            
        builder.Property(so => so.TotalAmount)
            .HasPrecision(18, 2);
            
        // Indexes
        builder.HasIndex(so => new { so.CompanyId, so.OrderNumber })
            .IsUnique();
            
        builder.HasIndex(so => so.CustomerId);
        builder.HasIndex(so => so.Status);
        builder.HasIndex(so => so.OrderDate);
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/SalesOrdersController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SalesOrdersController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSalesOrderCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetSalesOrderQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetSalesOrdersListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSalesOrderCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/confirm")]
    public async Task<IActionResult> Confirm(Guid id)
    {
        var command = new ConfirmSalesOrderCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/deliver")]
    public async Task<IActionResult> Deliver(Guid id, [FromBody] DeliverSalesOrderCommand command)
    {
        command.SalesOrderId = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok() : BadRequest(result);
    }
    
    [HttpPost("{id}/cancel")]
    public async Task<IActionResult> Cancel(Guid id)
    {
        var command = new CancelSalesOrderCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/salesorders` | Create sales order | Required |
| GET | `/api/salesorders/{id}` | Get sales order by ID | Required |
| GET | `/api/salesorders` | Get sales orders list (paginated) | Required |
| PUT | `/api/salesorders/{id}` | Update sales order | Required |
| POST | `/api/salesorders/{id}/confirm` | Confirm order (reserve stock) | Required |
| POST | `/api/salesorders/{id}/deliver` | Deliver order items | Required |
| POST | `/api/salesorders/{id}/cancel` | Cancel order (release stock) | Required |

## Frontend Components

### 1. Sales Order Service
**File**: `khidmah_inventory.client/src/app/core/services/sales-order-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class SalesOrderApiService {
  private apiUrl = '/api/salesorders';

  constructor(private http: HttpClient) {}

  getSalesOrders(params?: any): Observable<PagedResult<SalesOrderDto>> {
    return this.http.get<PagedResult<SalesOrderDto>>(this.apiUrl, { params });
  }

  getSalesOrder(id: string): Observable<SalesOrderDto> {
    return this.http.get<SalesOrderDto>(`${this.apiUrl}/${id}`);
  }

  createSalesOrder(order: CreateSalesOrderDto): Observable<SalesOrderDto> {
    return this.http.post<SalesOrderDto>(this.apiUrl, order);
  }

  updateSalesOrder(id: string, order: UpdateSalesOrderDto): Observable<SalesOrderDto> {
    return this.http.put<SalesOrderDto>(`${this.apiUrl}/${id}`, order);
  }

  confirmOrder(id: string): Observable<SalesOrderDto> {
    return this.http.post<SalesOrderDto>(`${this.apiUrl}/${id}/confirm`, {});
  }

  deliverOrder(id: string, deliverData: DeliverSalesOrderDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/deliver`, deliverData);
  }

  cancelOrder(id: string): Observable<SalesOrderDto> {
    return this.http.post<SalesOrderDto>(`${this.apiUrl}/${id}/cancel`, {});
  }
}
```

### 2. Sales Order List Component
- Display orders in table
- Filter by customer, status, date range
- Search functionality
- Actions: View, Edit, Confirm, Deliver, Cancel

### 3. Sales Order Form Component
- Create/Edit sales order
- Customer selection
- Warehouse selection
- Add/remove items
- Product selection with stock check
- Quantity, price, discount, tax inputs
- Auto-calculate totals
- Save as draft or confirm

### 4. Deliver Order Component
- Enter delivered quantities per item
- Submit delivery
- Update stock levels

## Workflow

### Create Sales Order Flow
```
User selects customer and warehouse → Add items → Check stock availability
→ Enter quantities and prices → Calculate totals → Save as Draft
→ API → Command Handler → Create entities → Save → Return DTO
```

### Confirm Order Flow
```
User clicks "Confirm" → API → Command Handler → Check stock availability
→ Reserve stock for each item → Update status to Confirmed → Save
→ Return updated DTO
```

### Deliver Order Flow
```
User selects order → Enter delivered quantities → Submit
→ API → Command Handler → Update delivered quantities
→ Create StockTransactions (StockOut) → Update StockLevels
→ Release reserved stock → Update order status → Save
```

## Testing Checklist

- [ ] Create sales order
- [ ] Create sales order with items
- [ ] Calculate totals correctly
- [ ] Update sales order (draft only)
- [ ] Confirm order (reserve stock)
- [ ] Confirm order with insufficient stock (should fail)
- [ ] Deliver order items
- [ ] Deliver partial quantities
- [ ] Stock levels update on delivery
- [ ] Cancel order (release reserved stock)
- [ ] Get sales orders list with filters
- [ ] Multi-tenancy isolation
- [ ] Frontend: Order form works correctly
- [ ] Frontend: Stock availability check works
- [ ] Frontend: Totals calculate correctly

