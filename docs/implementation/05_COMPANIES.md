# Companies Module Implementation

## Feature Overview

The Companies module manages multi-tenant company information. Each company is a separate tenant with its own data isolation. This module is critical for the multi-tenant architecture.

## Business Requirements

1. **Company Management**
   - Create, read, update companies
   - Company profile information
   - Subscription management
   - Company activation/deactivation

2. **Company Attributes**
   - Name, Legal Name
   - Tax ID, Registration Number
   - Contact information (email, phone, address)
   - Logo support
   - Currency and TimeZone settings
   - Subscription plan and expiry

3. **Multi-Tenancy**
   - Each company has isolated data
   - Users can belong to multiple companies
   - Default company for each user

## Domain Model

### Entity: Company
- **Location**: `Khidmah_Inventory.Domain/Entities/Company.cs`
- **Relationships**:
  - Has many: UserCompanies (many-to-many with Users)

### Key Properties
- `Name`, `LegalName`
- `TaxId`, `RegistrationNumber`
- `Email`, `PhoneNumber`, `Address`
- `LogoUrl`
- `Currency`, `TimeZone`
- `IsActive`
- `SubscriptionPlan`, `SubscriptionExpiresAt`

## Implementation Steps

### 1. Application Layer - Commands

#### Create Company Command
**File**: `Khidmah_Inventory.Application/Features/Companies/Commands/CreateCompany/CreateCompanyCommand.cs`

```csharp
public class CreateCompanyCommand : IRequest<Result<CompanyDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Currency { get; set; } = "USD";
    public string? TimeZone { get; set; }
}
```

**Handler**: 
- Validate name is unique
- Create Company entity (CompanyId = new Guid())
- Save to database
- Return CompanyDto

**Note**: Company creation is typically done by system admin or during registration.

#### Update Company Command
- Update company information
- Validate name uniqueness (excluding current company)

#### Activate/Deactivate Company Commands
- Toggle IsActive status
- Deactivation prevents login for all company users

### 2. Application Layer - Queries

#### Get Company Query
Load company information.

#### Get Current Company Query
**File**: `Khidmah_Inventory.Application/Features/Companies/Queries/GetCurrentCompany/GetCurrentCompanyQuery.cs`

```csharp
public class GetCurrentCompanyQuery : IRequest<Result<CompanyDto>>
{
}
```

**Handler**:
- Get company from ICurrentUserService
- Load company details
- Return CompanyDto

#### Get Companies List Query
For admin users to see all companies (with proper authorization).

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/Companies/CompanyDto.cs`

```csharp
public class CompanyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? LegalName { get; set; }
    public string? TaxId { get; set; }
    public string? RegistrationNumber { get; set; }
    public string? Email { get; set; }
    public string? PhoneNumber { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Country { get; set; }
    public string? PostalCode { get; set; }
    public string? LogoUrl { get; set; }
    public string? Currency { get; set; }
    public string? TimeZone { get; set; }
    public bool IsActive { get; set; }
    public string? SubscriptionPlan { get; set; }
    public DateTime? SubscriptionExpiresAt { get; set; }
    public int UserCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. Infrastructure Layer

#### Entity Configuration
**File**: `Khidmah_Inventory.Infrastructure/Data/Configurations/CompanyConfiguration.cs`

```csharp
public class CompanyConfiguration : IEntityTypeConfiguration<Company>
{
    public void Configure(EntityTypeBuilder<Company> builder)
    {
        builder.HasKey(c => c.Id);
        
        builder.Property(c => c.Name)
            .IsRequired()
            .HasMaxLength(200);
            
        builder.Property(c => c.Currency)
            .HasMaxLength(10)
            .HasDefaultValue("USD");
            
        // Indexes
        builder.HasIndex(c => c.Name)
            .IsUnique();
            
        builder.HasIndex(c => c.TaxId)
            .HasFilter("[TaxId] IS NOT NULL");
    }
}
```

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/CompaniesController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class CompaniesController : BaseApiController
{
    [HttpPost]
    [Authorize(Roles = "Admin")] // Only admins can create companies
    public async Task<IActionResult> Create([FromBody] CreateCompanyCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var query = new GetCurrentCompanyQuery();
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet("{id}")]
    [Authorize(Roles = "Admin")] // Only admins can view other companies
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetCompanyQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpPut("{id}")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCompanyCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/logo")]
    public async Task<IActionResult> UploadLogo(Guid id, IFormFile file)
    {
        var command = new UploadCompanyLogoCommand { Id = id, File = file };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/companies` | Create company | Admin only |
| GET | `/api/companies/current` | Get current company | Required |
| GET | `/api/companies/{id}` | Get company by ID | Admin only |
| PUT | `/api/companies/{id}` | Update company | Admin only |
| POST | `/api/companies/{id}/logo` | Upload company logo | Required |

## Frontend Components

### 1. Company Service
**File**: `khidmah_inventory.client/src/app/core/services/company-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class CompanyApiService {
  private apiUrl = '/api/companies';

  constructor(private http: HttpClient) {}

  getCurrentCompany(): Observable<CompanyDto> {
    return this.http.get<CompanyDto>(`${this.apiUrl}/current`);
  }

  getCompany(id: string): Observable<CompanyDto> {
    return this.http.get<CompanyDto>(`${this.apiUrl}/${id}`);
  }

  updateCompany(id: string, company: UpdateCompanyDto): Observable<CompanyDto> {
    return this.http.put<CompanyDto>(`${this.apiUrl}/${id}`, company);
  }

  uploadLogo(id: string, file: File): Observable<CompanyDto> {
    const formData = new FormData();
    formData.append('file', file);
    return this.http.post<CompanyDto>(`${this.apiUrl}/${id}/logo`, formData);
  }
}
```

### 2. Company Profile Component
- Display company information
- Edit company details
- Logo upload
- Subscription information

## Workflow

### Get Current Company Flow
```
User requests current company → API → Query Handler
→ Get CompanyId from ICurrentUserService → Load company
→ Return DTO → Display in UI
```

## Testing Checklist

- [ ] Create company (admin only)
- [ ] Get current company
- [ ] Update company (admin only)
- [ ] Upload company logo
- [ ] Prevent non-admin from creating companies
- [ ] Multi-tenancy: Users can only see their company
- [ ] Frontend: Company profile displays correctly
- [ ] Frontend: Logo upload works

