# Suppliers Module Implementation

## Feature Overview

The Suppliers module manages supplier/vendor information for purchase order management. Suppliers provide products to the company.

## Business Requirements

1. **Supplier Management**
   - Create, read, update, and delete suppliers
   - Supplier code and name
   - Contact information
   - Address details
   - Payment terms and credit limits
   - Active/Inactive status

2. **Supplier Attributes**
   - Name (required)
   - Code (optional, unique within company)
   - Contact person
   - Email, phone number
   - Address (street, city, state, country, postal code)
   - Tax ID
   - Payment terms
   - Credit limit
   - Balance (outstanding amount)
   - IsActive flag

## Domain Model

### Entity: Supplier
- **Location**: `Khidmah_Inventory.Domain/Entities/Supplier.cs`
- **Relationships**:
  - Has many: PurchaseOrders

### Key Properties
- `Name`, `Code`, `ContactPerson`
- `Email`, `PhoneNumber`
- `Address`, `City`, `State`, `Country`, `PostalCode`
- `TaxId`, `PaymentTerms`
- `CreditLimit`, `Balance`
- `IsActive`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Supplier Command
**File**: `Khidmah_Inventory.Application/Features/Suppliers/Commands/CreateSupplier/CreateSupplierCommand.cs`

```csharp
public class CreateSupplierCommand : IRequest<Result<SupplierDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? TaxId { get; set; }
    public string? PaymentTerms { get; set; }
    public decimal? CreditLimit { get; set; }
}
```

**Handler**: 
- Validate name is unique within company
- Validate code is unique if provided
- Validate email format if provided
- Create Supplier entity
- Save to database

**Validator**:
```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
RuleFor(x => x.Code).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Code));
RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0).When(x => x.CreditLimit.HasValue);
```

#### Update Supplier Command
- Similar validation
- Update entity

#### Activate/Deactivate Supplier Commands
- Toggle IsActive status

#### Delete Supplier Command
- Check if supplier has purchase orders
- Prevent deletion if has orders

### 2. Application Layer - Queries

#### Get Supplier Query
Load supplier with purchase order count and total order value.

#### Get Suppliers List Query
**File**: `Khidmah_Inventory.Application/Features/Suppliers/Queries/GetSuppliersList/GetSuppliersListQuery.cs`

```csharp
public class GetSuppliersListQuery : IRequest<Result<PagedResult<SupplierDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
}
```

**Handler**:
- Filter by CompanyId
- Apply search term (name, code, email)
- Filter by IsActive
- Apply pagination
- Return paged results

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/Suppliers/SupplierDto.cs`

```csharp
public class SupplierDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public string? ContactPerson { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? TaxId { get; set; }
    public string? PaymentTerms { get; set; }
    public decimal? CreditLimit { get; set; }
    public decimal Balance { get; set; }
    public bool IsActive { get; set; }
    public int PurchaseOrderCount { get; set; }
    public decimal TotalOrderValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/SupplierConfiguration.cs`

```csharp
public class SupplierConfiguration : IEntityTypeConfiguration<Supplier>
{
    public void Configure(EntityTypeBuilder<Supplier> builder)
    {
        builder.HasKey(s => s.Id);
        
        builder.Property(s => s.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(s => s.Code)
            .HasMaxLength(50);
            
        builder.Property(s => s.Balance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);
            
        // Indexes
        builder.HasIndex(s => new { s.CompanyId, s.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL");
            
        builder.HasIndex(s => s.Email)
            .HasFilter("[Email] IS NOT NULL");
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/SuppliersController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class SuppliersController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateSupplierCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetSupplierQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetSuppliersListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateSupplierCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteSupplierCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result);
    }
    
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateSupplierCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateSupplierCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/suppliers` | Create supplier | Required |
| GET | `/api/suppliers/{id}` | Get supplier by ID | Required |
| GET | `/api/suppliers` | Get suppliers list (paginated) | Required |
| PUT | `/api/suppliers/{id}` | Update supplier | Required |
| DELETE | `/api/suppliers/{id}` | Delete supplier | Required |
| POST | `/api/suppliers/{id}/activate` | Activate supplier | Required |
| POST | `/api/suppliers/{id}/deactivate` | Deactivate supplier | Required |

## Frontend Components

### 1. Supplier Service
**File**: `khidmah_inventory.client/src/app/core/services/supplier-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class SupplierApiService {
  private apiUrl = '/api/suppliers';

  constructor(private http: HttpClient) {}

  getSuppliers(params?: any): Observable<PagedResult<SupplierDto>> {
    return this.http.get<PagedResult<SupplierDto>>(this.apiUrl, { params });
  }

  getSupplier(id: string): Observable<SupplierDto> {
    return this.http.get<SupplierDto>(`${this.apiUrl}/${id}`);
  }

  createSupplier(supplier: CreateSupplierDto): Observable<SupplierDto> {
    return this.http.post<SupplierDto>(this.apiUrl, supplier);
  }

  updateSupplier(id: string, supplier: UpdateSupplierDto): Observable<SupplierDto> {
    return this.http.put<SupplierDto>(`${this.apiUrl}/${id}`, supplier);
  }

  deleteSupplier(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  activateSupplier(id: string): Observable<SupplierDto> {
    return this.http.post<SupplierDto>(`${this.apiUrl}/${id}/activate`, {});
  }

  deactivateSupplier(id: string): Observable<SupplierDto> {
    return this.http.post<SupplierDto>(`${this.apiUrl}/${id}/deactivate`, {});
  }
}
```

### 2. Supplier List Component
- Display suppliers in table
- Search functionality
- Filter by active status
- Show balance and credit limit
- Actions: View, Edit, Delete, Activate/Deactivate

### 3. Supplier Form Component
- Create/Edit supplier form
- Address fields
- Payment terms input
- Credit limit input
- Validation

## Workflow

### Create Supplier Flow
```
User fills form → Validate → Submit → API → Command Handler
→ Create entity → Save → Return DTO → Update list
```

## Testing Checklist

- [ ] Create supplier with valid data
- [ ] Create supplier with duplicate name (should fail)
- [ ] Create supplier with duplicate code (should fail)
- [ ] Update supplier
- [ ] Delete supplier with purchase orders (should fail)
- [ ] Delete supplier without orders
- [ ] Activate/Deactivate supplier
- [ ] Get suppliers list with pagination
- [ ] Search suppliers
- [ ] Multi-tenancy isolation
- [ ] Frontend: Supplier list displays correctly
- [ ] Frontend: Form validation works

