# Users Module Implementation

## Feature Overview

The Users module extends the authentication module to provide user management functionality. While authentication handles login/register, this module provides CRUD operations for user management, profile updates, and user-company relationships.

## Business Requirements

1. **User Management**
   - View, update, and delete users
   - User profile management
   - Password change
   - User activation/deactivation
   - Email confirmation

2. **User-Company Relationships**
   - Assign users to companies
   - Set default company
   - Activate/deactivate user-company relationships

3. **User Attributes**
   - Email, Username (already in User entity)
   - FirstName, LastName
   - PhoneNumber
   - IsActive
   - EmailConfirmed
   - LastLoginAt

## Domain Model

### Entity: User
- **Location**: `Khidmah_Inventory.Domain/Entities/User.cs`
- **Relationships**:
  - Many-to-many with Companies (via UserCompany)
  - Many-to-many with Roles (via UserRole)

### Key Properties
- `Email`, `UserName`, `PasswordHash`
- `FirstName`, `LastName`, `PhoneNumber`
- `IsActive`, `EmailConfirmed`
- `LastLoginAt`, `RefreshToken`, `RefreshTokenExpiryTime`

## Implementation Steps

### 1. Application Layer - Commands

#### Update User Profile Command
**File**: `Khidmah_Inventory.Application/Features/Users/Commands/UpdateUserProfile/UpdateUserProfileCommand.cs`

```csharp
public class UpdateUserProfileCommand : IRequest<Result<UserDto>>
{
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
}
```

**Handler**: 
- Load user by ID
- Verify user belongs to current company (or is admin)
- Update profile using entity method
- Save changes

#### Change Password Command
**File**: `Khidmah_Inventory.Application/Features/Users/Commands/ChangePassword/ChangePasswordCommand.cs`

```csharp
public class ChangePasswordCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public string CurrentPassword { get; set; } = string.Empty;
    public string NewPassword { get; set; } = string.Empty;
}
```

**Handler**:
- Load user
- Verify current password
- Hash new password
- Update password
- Save changes

#### Activate/Deactivate User Commands
- Toggle IsActive status
- Prevent deactivation of own account

#### Assign User to Company Command
**File**: `Khidmah_Inventory.Application/Features/Users/Commands/AssignUserToCompany/AssignUserToCompanyCommand.cs`

```csharp
public class AssignUserToCompanyCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid CompanyId { get; set; }
    public bool IsDefault { get; set; } = false;
}
```

**Handler**:
- Check if user-company relationship exists
- Create or update UserCompany
- If IsDefault, unset other defaults for user
- Save changes

### 2. Application Layer - Queries

#### Get User Query
Load user with roles and companies.

#### Get Users List Query
**File**: `Khidmah_Inventory.Application/Features/Users/Queries/GetUsersList/GetUsersListQuery.cs`

```csharp
public class GetUsersListQuery : IRequest<Result<PagedResult<UserDto>>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
    public string? SearchTerm { get; set; }
    public bool? IsActive { get; set; }
}
```

**Handler**:
- Filter by CompanyId (users in current company)
- Apply search term
- Filter by IsActive
- Include roles and companies
- Apply pagination

#### Get Current User Query
**File**: `Khidmah_Inventory.Application/Features/Users/Queries/GetCurrentUser/GetCurrentUserQuery.cs`

```csharp
public class GetCurrentUserQuery : IRequest<Result<UserDto>>
{
}
```

**Handler**:
- Get current user ID from ICurrentUserService
- Load user with roles and permissions
- Return UserDto

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/Users/UserDto.cs`

```csharp
public class UserDto
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string UserName { get; set; } = string.Empty;
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string? PhoneNumber { get; set; }
    public bool IsActive { get; set; }
    public bool EmailConfirmed { get; set; }
    public DateTime? LastLoginAt { get; set; }
    public List<string> Roles { get; set; } = new();
    public List<string> Permissions { get; set; } = new();
    public List<CompanyDto> Companies { get; set; } = new();
    public Guid? DefaultCompanyId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
```

### 4. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/UsersController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : BaseApiController
{
    [HttpGet("current")]
    public async Task<IActionResult> GetCurrent()
    {
        var query = new GetCurrentUserQuery();
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetUserQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList([FromQuery] GetUsersListQuery query)
    {
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}/profile")]
    public async Task<IActionResult> UpdateProfile(Guid id, [FromBody] UpdateUserProfileCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid id, [FromBody] ChangePasswordCommand command)
    {
        command.UserId = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok() : BadRequest(result);
    }
    
    [HttpPost("{id}/activate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Activate(Guid id)
    {
        var command = new ActivateUserCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/deactivate")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> Deactivate(Guid id)
    {
        var command = new DeactivateUserCommand { Id = id };
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/assign-company")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> AssignToCompany(Guid id, [FromBody] AssignUserToCompanyCommand command)
    {
        command.UserId = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok() : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| GET | `/api/users/current` | Get current user | Required |
| GET | `/api/users/{id}` | Get user by ID | Required |
| GET | `/api/users` | Get users list (paginated) | Required |
| PUT | `/api/users/{id}/profile` | Update user profile | Required |
| POST | `/api/users/{id}/change-password` | Change password | Required |
| POST | `/api/users/{id}/activate` | Activate user | Admin only |
| POST | `/api/users/{id}/deactivate` | Deactivate user | Admin only |
| POST | `/api/users/{id}/assign-company` | Assign user to company | Admin only |

## Frontend Components

### 1. User Service
**File**: `khidmah_inventory.client/src/app/core/services/user-api.service.ts`

```typescript
@Injectable({ providedIn: 'root' })
export class UserApiService {
  private apiUrl = '/api/users';

  constructor(private http: HttpClient) {}

  getCurrentUser(): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/current`);
  }

  getUsers(params?: any): Observable<PagedResult<UserDto>> {
    return this.http.get<PagedResult<UserDto>>(this.apiUrl, { params });
  }

  getUser(id: string): Observable<UserDto> {
    return this.http.get<UserDto>(`${this.apiUrl}/${id}`);
  }

  updateProfile(id: string, profile: UpdateUserProfileDto): Observable<UserDto> {
    return this.http.put<UserDto>(`${this.apiUrl}/${id}/profile`, profile);
  }

  changePassword(id: string, passwords: ChangePasswordDto): Observable<void> {
    return this.http.post<void>(`${this.apiUrl}/${id}/change-password`, passwords);
  }
}
```

### 2. User List Component
- Display users in table
- Search functionality
- Filter by active status
- Actions: View, Edit, Activate/Deactivate

### 3. User Profile Component
- Display user information
- Edit profile form
- Change password form
- Company assignments

## Workflow

### Update Profile Flow
```
User edits profile → Validate → Submit → API → Command Handler
→ Load user → Update entity → Save → Return DTO → Update UI
```

### Change Password Flow
```
User enters current and new password → Validate → Submit
→ API → Command Handler → Verify current password
→ Hash new password → Update → Save → Return success
```

## Testing Checklist

- [ ] Get current user
- [ ] Get users list
- [ ] Update user profile
- [ ] Change password with correct current password
- [ ] Change password with incorrect current password (should fail)
- [ ] Activate/Deactivate user (admin only)
- [ ] Assign user to company
- [ ] Prevent deactivating own account
- [ ] Multi-tenancy: Users can only see users in their company
- [ ] Frontend: Profile form works correctly
- [ ] Frontend: Password change works

