# Customers Module Implementation

## Feature Overview

The Customers module manages customer information for sales order management. Customers purchase products from the company.

## Business Requirements

1. **Customer Management**
   - Create, read, update, and delete customers
   - Customer code and name
   - Contact information
   - Address details
   - Payment terms and credit limits
   - Active/Inactive status

2. **Customer Attributes**
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

### Entity: Customer
- **Location**: `Khidmah_Inventory.Domain/Entities/Customer.cs`
- **Relationships**:
  - Has many: SalesOrders

### Key Properties
- `Name`, `Code`, `ContactPerson`
- `Email`, `PhoneNumber`
- `Address`, `City`, `State`, `Country`, `PostalCode`
- `TaxId`, `PaymentTerms`
- `CreditLimit`, `Balance`
- `IsActive`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Customer Command
**File**: `Khidmah_Inventory.Application/Features/Customers/Commands/CreateCustomer/CreateCustomerCommand.cs`

```csharp
public class CreateCustomerCommand : IRequest<Result<CustomerDto>>
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
- Create Customer entity
- Save to database

**Validator**:
```csharp
RuleFor(x => x.Name).NotEmpty().MaximumLength(200);
RuleFor(x => x.Code).MaximumLength(50).When(x => !string.IsNullOrEmpty(x.Code));
RuleFor(x => x.Email).EmailAddress().When(x => !string.IsNullOrEmpty(x.Email));
RuleFor(x => x.CreditLimit).GreaterThanOrEqualTo(0).When(x => x.CreditLimit.HasValue);
```

#### Update Customer Command
- Similar validation
- Update entity

#### Activate/Deactivate Customer Commands
- Toggle IsActive status

#### Delete Customer Command
- Check if customer has sales orders
- Prevent deletion if has orders

### 2. Application Layer - Queries

#### Get Customer Query
Load customer with sales order count and total order value.

#### Get Customers List Query
**File**: `Khidmah_Inventory.Application/Features/Customers/Queries/GetCustomersList/GetCustomersListQuery.cs`

```csharp
public class GetCustomersListQuery : IRequest<Result<PagedResult<CustomerDto>>>
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

**File**: `Khidmah_Inventory.Application/Features/Customers/CustomerDto.cs`

```csharp
public class CustomerDto
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
    public int SalesOrderCount { get; set; }
    public decimal TotalOrderValue { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/CustomerConfiguration.cs`

```csharp
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(c => c.Code)
            .HasMaxLength(50);
            
        builder.Property(c => c.Balance)
            .HasPrecision(18, 2)
            .HasDefaultValue(0);
            
        // Indexes
        builder.HasIndex(c => new { c.CompanyId, c.Code })
            .IsUnique()
            .HasFilter("[Code] IS NOT NULL");
            
        builder.HasIndex(c => c.Email)
            .HasFilter("[Email] IS NOT NULL");
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/CustomersController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CustomersController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateCustomerCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetCustomerQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetCustomersListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCustomerCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCustomerCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? NoContent() : BadRequest(result);
    }
    
    [HttpPost("{id}/activate")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateCustomerCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/deactivate")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateCustomerCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/customers` | Create customer | Required |
| GET | `/api/customers/{id}` | Get customer by ID | Required |
| GET | `/api/customers` | Get customers list (paginated) | Required |
| PUT | `/api/customers/{id}` | Update customer | Required |
| DELETE | `/api/customers/{id}` | Delete customer | Required |
| POST | `/api/customers/{id}/activate` | Activate customer | Required |
| POST | `/api/customers/{id}/deactivate` | Deactivate customer | Required |

## Frontend Components

### 1. Customer Service
**File**: `khidmah_inventory.client/src/app/core/services/customer-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class CustomerApiService {
  private apiUrl = '/api/customers';

  constructor(private http: HttpClient) {}

  getCustomers(params?: any): Observable<PagedResult<CustomerDto>> {
    return this.http.get<PagedResult<CustomerDto>>(this.apiUrl, { params });
  }

  getCustomer(id: string): Observable<CustomerDto> {
    return this.http.get<CustomerDto>(`${this.apiUrl}/${id}`);
  }

  createCustomer(customer: CreateCustomerDto): Observable<CustomerDto> {
    return this.http.post<CustomerDto>(this.apiUrl, customer);
  }

  updateCustomer(id: string, customer: UpdateCustomerDto): Observable<CustomerDto> {
    return this.http.put<CustomerDto>(`${this.apiUrl}/${id}`, customer);
  }

  deleteCustomer(id: string): Observable<void> {
    return this.http.delete<void>(`${this.apiUrl}/${id}`);
  }

  activateCustomer(id: string): Observable<CustomerDto> {
    return this.http.post<CustomerDto>(`${this.apiUrl}/${id}/activate`, {});
  }

  deactivateCustomer(id: string): Observable<CustomerDto> {
    return this.http.post<CustomerDto>(`${this.apiUrl}/${id}/deactivate`, {});
  }
}
```

### 2. Customer List Component
- Display customers in table
- Search functionality
- Filter by active status
- Show balance and credit limit
- Actions: View, Edit, Delete, Activate/Deactivate

### 3. Customer Form Component
- Create/Edit customer form
- Address fields
- Payment terms input
- Credit limit input
- Validation

## Workflow

### Create Customer Flow
```
User fills form → Validate → Submit → API → Command Handler
→ Create entity → Save → Return DTO → Update list
```

## Testing Checklist

- [ ] Create customer with valid data
- [ ] Create customer with duplicate name (should fail)
- [ ] Create customer with duplicate code (should fail)
- [ ] Update customer
- [ ] Delete customer with sales orders (should fail)
- [ ] Delete customer without orders
- [ ] Activate/Deactivate customer
- [ ] Get customers list with pagination
- [ ] Search customers
- [ ] Multi-tenancy isolation
- [ ] Frontend: Customer list displays correctly
- [ ] Frontend: Form validation works

