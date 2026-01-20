# Roles & Permissions Module Implementation

## Feature Overview

The Roles & Permissions module implements Role-Based Access Control (RBAC) and Permission-Based Access Control. It manages roles, permissions, and their assignments to users.

## Business Requirements

1. **Role Management**
   - Create, read, update, and delete roles
   - Role name and description
   - System roles (cannot be modified)
   - Assign permissions to roles

2. **Permission Management**
   - View available permissions
   - Permissions organized by module
   - Permission actions (Create, Read, Update, Delete, etc.)

3. **User-Role Assignment**
   - Assign roles to users
   - Users can have multiple roles
   - Roles are company-specific

4. **Role-Permission Assignment**
   - Assign permissions to roles
   - Roles can have multiple permissions
   - Permissions are company-specific

## Domain Model

### Entity: Role
- **Location**: `Khidmah_Inventory.Domain/Entities/Role.cs`
- **Relationships**:
  - Has many: UserRoles (many-to-many with Users)
  - Has many: RolePermissions (many-to-many with Permissions)

### Entity: Permission
- **Location**: `Khidmah_Inventory.Domain/Entities/Permission.cs`
- **Relationships**:
  - Has many: RolePermissions (many-to-many with Roles)

### Entity: UserRole
- **Location**: `Khidmah_Inventory.Domain/Entities/UserRole.cs`
- **Relationships**:
  - Belongs to: User, Role

### Entity: RolePermission
- **Location**: `Khidmah_Inventory.Domain/Entities/RolePermission.cs`
- **Relationships**:
  - Belongs to: Role, Permission

## Implementation Steps

### 1. Application Layer - Commands

#### Create Role Command
**File**: `Khidmah_Inventory.Application/Features/Roles/Commands/CreateRole/CreateRoleCommand.cs`

```csharp
public class CreateRoleCommand : IRequest<Result<RoleDto>>
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
}
```

**Handler**: 
- Validate name is unique within company
- Create Role entity
- Assign permissions
- Save to database

#### Update Role Command
- Similar to create
- Prevent updating system roles
- Update permissions

#### Delete Role Command
- Check if role is assigned to users
- Prevent deletion if assigned
- Prevent deletion of system roles

#### Assign Permissions to Role Command
**File**: `Khidmah_Inventory.Application/Features/Roles/Commands/AssignPermissionsToRole/AssignPermissionsToRoleCommand.cs`

```csharp
public class AssignPermissionsToRoleCommand : IRequest<Result>
{
    public Guid RoleId { get; set; }
    public List<Guid> PermissionIds { get; set; } = new();
}
```

**Handler**:
- Load role
- Remove existing permissions
- Add new permissions
- Save changes

#### Assign Role to User Command
**File**: `Khidmah_Inventory.Application/Features/Roles/Commands/AssignRoleToUser/AssignRoleToUserCommand.cs`

```csharp
public class AssignRoleToUserCommand : IRequest<Result>
{
    public Guid UserId { get; set; }
    public Guid RoleId { get; set; }
}
```

**Handler**:
- Check if user-role relationship exists
- Create UserRole entity
- Save changes

### 2. Application Layer - Queries

#### Get Role Query
Load role with permissions and user count.

#### Get Roles List Query
**File**: `Khidmah_Inventory.Application/Features/Roles/Queries/GetRolesList/GetRolesListQuery.cs`

```csharp
public class GetRolesListQuery : IRequest<Result<List<RoleDto>>>
{
}
```

**Handler**:
- Filter by CompanyId
- Include permission count
- Include user count
- Return list

#### Get Permissions List Query
**File**: `Khidmah_Inventory.Application/Features/Permissions/Queries/GetPermissionsList/GetPermissionsListQuery.cs`

```csharp
public class GetPermissionsListQuery : IRequest<Result<List<PermissionDto>>>
{
    public string? Module { get; set; }
}
```

**Handler**:
- Filter by CompanyId
- Filter by Module if provided
- Group by module
- Return list

### 3. DTOs

**File**: `Khidmah_Inventory.Application/Features/Roles/RoleDto.cs`

```csharp
public class RoleDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public bool IsSystemRole { get; set; }
    public int UserCount { get; set; }
    public int PermissionCount { get; set; }
    public List<PermissionDto> Permissions { get; set; } = new();
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}

public class PermissionDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Module { get; set; } = string.Empty;
    public string Action { get; set; } = string.Empty;
}
```

### 4. Infrastructure Layer

#### Entity Configurations
Similar to other entities, with proper indexes and relationships.

### 5. API Layer

#### Controller
**File**: `Khidmah_Inventory.API/Controllers/RolesController.cs`

```csharp
[ApiController]
[Route("api/[controller]")]
[Authorize(Roles = "Admin")]
public class RolesController : BaseApiController
{
    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleCommand command)
    {
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id)
    {
        var query = new GetRoleQuery { Id = id };
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : NotFound(result);
    }
    
    [HttpGet]
    public async Task<IActionResult> GetList()
    {
        var query = new GetRolesListQuery();
        var result = await Mediator.Send(query);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateRoleCommand command)
    {
        command.Id = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok(result.Data) : BadRequest(result);
    }
    
    [HttpPost("{id}/permissions")]
    public async Task<IActionResult> AssignPermissions(Guid id, [FromBody] AssignPermissionsToRoleCommand command)
    {
        command.RoleId = id;
        var result = await Mediator.Send(command);
        return result.Succeeded ? Ok() : BadRequest(result);
    }
}
```

## API Endpoints

| Method | Endpoint | Description | Auth |
|--------|----------|-------------|------|
| POST | `/api/roles` | Create role | Admin only |
| GET | `/api/roles/{id}` | Get role by ID | Admin only |
| GET | `/api/roles` | Get roles list | Admin only |
| PUT | `/api/roles/{id}` | Update role | Admin only |
| DELETE | `/api/roles/{id}` | Delete role | Admin only |
| POST | `/api/roles/{id}/permissions` | Assign permissions | Admin only |
| GET | `/api/permissions` | Get permissions list | Admin only |

## Frontend Components

### 1. Role Service
Similar to other services, with methods for CRUD operations.

### 2. Role List Component
- Display roles in table
- Show user count and permission count
- Actions: View, Edit, Delete, Manage Permissions

### 3. Role Form Component
- Create/Edit role form
- Permission selection (checkboxes grouped by module)
- Validation

### 4. Permission Management Component
- Display permissions grouped by module
- Filter by module
- Show which roles have each permission

## Workflow

### Create Role Flow
```
User fills form → Select permissions → Submit
→ API → Command Handler → Create role → Assign permissions
→ Save → Return DTO → Update list
```

## Testing Checklist

- [ ] Create role
- [ ] Create role with permissions
- [ ] Update role
- [ ] Prevent updating system role
- [ ] Delete role (not assigned to users)
- [ ] Prevent deleting role assigned to users
- [ ] Assign permissions to role
- [ ] Assign role to user
- [ ] Get roles list
- [ ] Get permissions list
- [ ] Filter permissions by module
- [ ] Multi-tenancy isolation
- [ ] Frontend: Role form works correctly
- [ ] Frontend: Permission selection works

