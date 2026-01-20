# API Endpoint Permissions

This document maps all API endpoints to their required permissions and roles for authorization.

## Permission Format

Permissions follow the pattern: `{Module}:{Action}`

**Modules**: Auth, Products, Companies, Settings, Theme, System, Reports, etc.  
**Actions**: Create, Read, Update, Delete, List, Export, Import, etc.

---

## Authentication Endpoints

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/auth/login` | POST | `[AllowAnonymous]` | None | None | Public endpoint |
| `/api/auth/register` | POST | Required | `Admin` | `Auth:Create` | Only admins can register users |

---

## Theme Endpoints

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/theme/user` | GET | Required | Any | `Theme:Read` | User's own theme |
| `/api/theme/user` | POST | Required | Any | `Theme:Update` | User's own theme |
| `/api/theme/global` | GET | Required | Any | `Theme:Read` | Global theme |
| `/api/theme/global` | POST | Required | `Admin` | `Theme:Update` | Only admins can change global theme |
| `/api/theme/logo` | POST | Required | `Admin` | `Theme:Update` | Only admins can upload logos |

---

## Settings Endpoints

### Company Settings

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/settings/company` | GET | Required | Any | `Settings:Company:Read` | Company settings |
| `/api/settings/company` | POST | Required | `Admin` | `Settings:Company:Update` | Only admins can update company settings |

### User Settings

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/settings/user` | GET | Required | Any | `Settings:User:Read` | User's own settings |
| `/api/settings/user` | POST | Required | Any | `Settings:User:Update` | User's own settings |

### System Settings

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/settings/system` | GET | Required | `Admin` | `Settings:System:Read` | Only admins can view system settings |
| `/api/settings/system` | POST | Required | `Admin` | `Settings:System:Update` | Only admins can update system settings |

### Notification Settings

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/settings/notifications` | GET | Required | Any | `Settings:Notification:Read` | Notification settings |
| `/api/settings/notifications` | POST | Required | `Admin` | `Settings:Notification:Update` | Only admins can update notification settings |

### UI Settings

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/settings/ui` | GET | Required | Any | `Settings:UI:Read` | User's own UI settings |
| `/api/settings/ui` | POST | Required | Any | `Settings:UI:Update` | User's own UI settings |

### Report Settings

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/settings/reports` | GET | Required | Any | `Settings:Report:Read` | Report settings |
| `/api/settings/reports` | POST | Required | `Admin` | `Settings:Report:Update` | Only admins can update report settings |

---

## Roles Endpoints

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/roles` | GET | Required | Any | `Roles:List` | Get all roles |
| `/api/roles/{id}` | GET | Required | Any | `Roles:Read` | Get role by ID |
| `/api/roles` | POST | Required | Any | `Roles:Create` | Create new role |
| `/api/roles/{id}` | PUT | Required | Any | `Roles:Update` | Update role (cannot update system roles) |
| `/api/roles/{id}` | DELETE | Required | Any | `Roles:Delete` | Delete role (cannot delete system roles or roles assigned to users) |
| `/api/roles/{roleId}/assign-user/{userId}` | POST | Required | Any | `Roles:Assign` | Assign role to user |
| `/api/roles/{roleId}/remove-user/{userId}` | DELETE | Required | Any | `Roles:Assign` | Remove role from user |

## Permissions Endpoints

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/permissions` | GET | Required | Any | `Permissions:Read` | Get all permissions (optionally filtered by module) |

---

## Products Endpoints (To be implemented)

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/products` | GET | Required | Any | `Products:List` | List products with filtering |
| `/api/products/{id}` | GET | Required | Any | `Products:Read` | Get product by ID |
| `/api/products` | POST | Required | Any | `Products:Create` | Create new product |
| `/api/products/{id}` | PUT | Required | Any | `Products:Update` | Update product |
| `/api/products/{id}` | DELETE | Required | Any | `Products:Delete` | Delete product (soft delete) |

---

## Companies Endpoints (To be implemented)

| Endpoint | Method | Authentication | Roles | Permissions | Notes |
|----------|--------|----------------|-------|-------------|-------|
| `/api/companies` | GET | Required | `Admin` | `Companies:List` | List companies (admin only) |
| `/api/companies/{id}` | GET | Required | Any | `Companies:Read` | Get company by ID |
| `/api/companies` | POST | Required | `Admin` | `Companies:Create` | Create company (admin only) |
| `/api/companies/{id}` | PUT | Required | `Admin` | `Companies:Update` | Update company (admin only) |

---

## Permission Categories

### Auth Module
- `Auth:Create` - Create/register users

### Roles Module
- `Roles:Create` - Create roles
- `Roles:Read` - View roles
- `Roles:Update` - Update roles
- `Roles:Delete` - Delete roles
- `Roles:List` - List roles
- `Roles:Assign` - Assign/remove roles from users

### Permissions Module
- `Permissions:Read` - View permissions
- `Permissions:List` - List permissions

### Products Module
- `Products:Create` - Create products
- `Products:Read` - View products
- `Products:Update` - Update products
- `Products:Delete` - Delete products
- `Products:List` - List products
- `Products:Export` - Export products

### Companies Module
- `Companies:Create` - Create companies
- `Companies:Read` - View companies
- `Companies:Update` - Update companies
- `Companies:Delete` - Delete companies
- `Companies:List` - List companies

### Settings Module
- `Settings:Company:Read` - View company settings
- `Settings:Company:Update` - Update company settings
- `Settings:User:Read` - View user settings
- `Settings:User:Update` - Update user settings
- `Settings:System:Read` - View system settings
- `Settings:System:Update` - Update system settings
- `Settings:Notification:Read` - View notification settings
- `Settings:Notification:Update` - Update notification settings
- `Settings:UI:Read` - View UI settings
- `Settings:UI:Update` - Update UI settings
- `Settings:Report:Read` - View report settings
- `Settings:Report:Update` - Update report settings

### Theme Module
- `Theme:Read` - View themes
- `Theme:Update` - Update themes

### System Module
- `System:Admin` - System administration
- `System:Audit` - View audit logs
- `System:Backup` - Backup/restore data

---

## Role Hierarchy

### Admin
- Full access to all endpoints
- Can manage users, roles, permissions
- Can update system settings
- Can manage companies

### Manager
- Can manage products, inventory, orders
- Can view reports
- Cannot manage users or system settings

### User
- Can view and update own data
- Can perform assigned tasks
- Limited access based on permissions

---

## Implementation Notes

### Current Implementation
- ✅ All endpoints use `[Authorize]` attribute (requires authentication)
- ✅ All endpoints use `[AuthorizePermission("Module:Action")]` for permission-based access
- ✅ Permission-based authorization fully implemented
- ✅ Custom `AuthorizePermissionAttribute` created
- ✅ Permission handler checks permissions from JWT claims
- ✅ Frontend directives and services for permission-based UI control

---

## Authorization Attributes

### Current Usage
```csharp
[Authorize]                                    // Requires authentication
[Authorize(Roles = "Admin")]                  // Requires Admin role (legacy, use permissions)
[AllowAnonymous]                               // No authentication required
[AuthorizePermission("Products:Create")]       // Requires specific permission (IMPLEMENTED)
```

---

## Permission Checking Strategy

### Option 1: Attribute-Based (Recommended)
```csharp
[HttpPost]
[Authorize(Permission = "Products:Create")]
public async Task<IActionResult> Create([FromBody] CreateProductCommand command)
{
    // Handler automatically checks permission
}
```

### Option 2: Handler-Based
```csharp
public async Task<Result<ProductDto>> Handle(CreateProductCommand request, ...)
{
    if (!_currentUser.HasPermission("Products:Create"))
    {
        return Result<ProductDto>.Failure("Insufficient permissions");
    }
    // ... rest of handler
}
```

### Option 3: Middleware-Based
- Global permission checking middleware
- Checks permissions before reaching handlers
- Returns 403 if permission denied

---

## Permission Matrix

| Endpoint | Admin | Manager | User | Guest |
|----------|-------|---------|------|-------|
| POST /api/auth/login | ✅ | ✅ | ✅ | ✅ |
| POST /api/auth/register | ✅ | ❌ | ❌ | ❌ |
| GET /api/settings/company | ✅ | ✅ | ✅ | ❌ |
| POST /api/settings/company | ✅ | ❌ | ❌ | ❌ |
| GET /api/settings/user | ✅ | ✅ | ✅ | ❌ |
| POST /api/settings/user | ✅ | ✅ | ✅ | ❌ |
| GET /api/settings/system | ✅ | ❌ | ❌ | ❌ |
| POST /api/settings/system | ✅ | ❌ | ❌ | ❌ |
| GET /api/theme/user | ✅ | ✅ | ✅ | ❌ |
| POST /api/theme/global | ✅ | ❌ | ❌ | ❌ |

---

## Summary

- ✅ **Authentication**: JWT-based, all endpoints require authentication (except login)
- ✅ **Permission-Based**: All endpoints use permission-based authorization
- ✅ **Role-Based**: Roles are used for grouping permissions
- ✅ **Multi-Tenancy**: All endpoints automatically filter by CompanyId
- ✅ **Frontend UI Control**: Directives and services for permission-based UI
- ✅ **Complete Implementation**: Backend to frontend fully implemented

---

## Implementation Status

✅ **Completed**:
1. ✅ Permission-based authorization attributes implemented
2. ✅ Permission checking in authorization handler
3. ✅ Permission management UI created
4. ✅ Frontend directives for permission-based UI control
5. ✅ All endpoints protected with permissions
6. ✅ Role management UI created
7. ✅ Permission service for frontend permission checks

🚧 **Next Steps**:
1. Create permission seeding/migration for default permissions
2. Create default roles (Admin, Manager, User) with permissions
3. Add user-role assignment UI
4. Add permission audit logging
5. Add permission caching for performance

