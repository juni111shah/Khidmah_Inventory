# Permission Mapping Guide

This document provides a comprehensive mapping of all permissions used throughout the application, including their usage in routes, components, and API endpoints.

## Permission Format

All permissions follow the pattern: `{Module}:{Action}` or `{Module}:{SubModule}:{Action}`

### Actions
- `Create` - Create new records
- `Read` - View/read records
- `Update` - Update existing records
- `Delete` - Delete records
- `List` - List/view multiple records
- `Activate` - Activate records
- `Deactivate` - Deactivate records
- `ChangePassword` - Change user password
- `Assign` - Assign roles/permissions

## Module Permissions

### Dashboard
- `Dashboard:Read` - Access dashboard page

**Usage:**
- Route: `/dashboard` (AuthGuard only)
- Navigation: Dashboard menu item

### Users Module

#### Permissions
- `Users:List` - View list of users
- `Users:Read` - View individual user details
- `Users:Create` - Create new users
- `Users:Update` - Update user information
- `Users:Delete` - Delete users
- `Users:Activate` - Activate users
- `Users:Deactivate` - Deactivate users
- `Users:ChangePassword` - Change user password

#### Routes
- `/users` - Requires `Users:List`
- `/users/:id` - Requires `Users:Read`
- `/users/:id/edit` - Requires `Users:Update`
- `/users/profile` - Requires authentication only (users can view their own profile)

#### Components
- `UsersListComponent`:
  - View button: `Users:Read`
  - Edit button: `Users:Update`
  - Activate button: `Users:Activate`
  - Deactivate button: `Users:Deactivate`
- `UserProfileComponent`:
  - Profile tab: `Users:Read`
  - Password tab: `Users:Update`
  - Save profile button: `Users:Update`
  - Change password button: `Users:ChangePassword`

#### API Endpoints
- `GET /api/users/current` - `Users:Read`
- `GET /api/users/{id}` - `Users:Read`
- `POST /api/users/list` - `Users:List`
- `PUT /api/users/{id}/profile` - `Users:Update`
- `POST /api/users/{id}/change-password` - `Users:ChangePassword`
- `POST /api/users/{id}/activate` - `Users:Activate`
- `POST /api/users/{id}/deactivate` - `Users:Deactivate`

### Roles Module

#### Permissions
- `Roles:List` - View list of roles
- `Roles:Read` - View individual role details
- `Roles:Create` - Create new roles
- `Roles:Update` - Update role information
- `Roles:Delete` - Delete roles
- `Roles:Assign` - Assign roles to users

#### Routes
- `/roles` - Requires `Roles:List`
- `/roles/new` - Requires `Roles:Create`
- `/roles/:id` - Requires `Roles:Read`
- `/roles/:id/edit` - Requires `Roles:Update`

#### Components
- `RolesListComponent`:
  - Create button: `Roles:Create`
  - View action: `Roles:Read`
  - Edit action: `Roles:Update`
  - Delete action: `Roles:Delete`
- `RoleFormComponent`:
  - Save button: `Roles:Create` (new) or `Roles:Update` (edit)

#### API Endpoints
- `GET /api/roles` - `Roles:List`
- `GET /api/roles/{id}` - `Roles:Read`
- `POST /api/roles` - `Roles:Create`
- `PUT /api/roles/{id}` - `Roles:Update`
- `DELETE /api/roles/{id}` - `Roles:Delete`

### Permissions Module

#### Permissions
- `Permissions:Read` - View permissions list
- `Permissions:List` - List all permissions

#### API Endpoints
- `GET /api/permissions` - `Permissions:Read`
- `GET /api/permissions?module={module}` - `Permissions:Read`

### Settings Module

#### Permissions
- `Settings:Read` - General settings access
- `Settings:Company:Read` - View company settings
- `Settings:Company:Update` - Update company settings
- `Settings:User:Read` - View user profile settings
- `Settings:User:Update` - Update user profile settings
- `Settings:UserProfile:Read` - View user profile settings (alternative)
- `Settings:UserProfile:Update` - Update user profile settings (alternative)
- `Settings:System:Read` - View system settings
- `Settings:System:Update` - Update system settings
- `Settings:Notification:Read` - View notification settings
- `Settings:Notification:Update` - Update notification settings
- `Settings:Notifications:Read` - View notification settings (alternative)
- `Settings:Notifications:Update` - Update notification settings (alternative)
- `Settings:UI:Read` - View UI settings
- `Settings:UI:Update` - Update UI settings
- `Settings:Report:Read` - View report settings
- `Settings:Report:Update` - Update report settings
- `Settings:Reports:Read` - View report settings (alternative)
- `Settings:Reports:Update` - Update report settings (alternative)
- `Settings:Layout:Read` - View layout settings
- `Settings:Layout:Update` - Update layout settings
- `Settings:Theme:Read` - View theme settings
- `Settings:Theme:Update` - Update theme settings
- `Settings:UIComponents:Read` - View UI component settings
- `Settings:UIComponents:Update` - Update UI component settings

#### Routes
- `/settings` - Requires any of: `Settings:Company:Read`, `Settings:User:Read`, `Settings:System:Read`, `Settings:Notification:Read`, `Settings:UI:Read`, `Settings:Report:Read`

#### Components
- `SettingsComponent`:
  - Company section: `Settings:Company:Read` (view), `Settings:Company:Update` (save)
  - User section: `Settings:UserProfile:Read` (view), `Settings:UserProfile:Update` (save)
  - System section: `Settings:System:Read` (view), `Settings:System:Update` (save)
  - Notification section: `Settings:Notifications:Read` (view), `Settings:Notifications:Update` (save)
  - UI section: `Settings:UI:Read` (view), `Settings:UI:Update` (save)
  - Report section: `Settings:Reports:Read` (view), `Settings:Reports:Update` (save)
  - Theme section: `Settings:Theme:Read` (view), `Settings:Theme:Update` (save)
  - Layout section: `Settings:Layout:Read` (view), `Settings:Layout:Update` (save)
  - UI Components section: `Settings:UIComponents:Read` (view), `Settings:UIComponents:Update` (save)

#### Navigation
- Settings menu item: Requires any of the settings read permissions
- Settings sub-items: Each requires its specific read permission

#### API Endpoints
- `GET /api/settings/{type}` - Requires corresponding `Settings:{Type}:Read`
- `PUT /api/settings/{type}` - Requires corresponding `Settings:{Type}:Update`

### Theme Module

#### Permissions
- `Theme:Read` - View theme settings
- `Theme:Update` - Update theme settings

#### API Endpoints
- `GET /api/theme` - `Theme:Read`
- `PUT /api/theme` - `Theme:Update`
- `POST /api/theme/logo` - `Theme:Update`

### Auth Module

#### Permissions
- `Auth:Create` - Register new users (admin only)

#### Routes
- `/login` - Public route
- `/register` - Requires `Auth:Create` (or can be public based on configuration)

#### API Endpoints
- `POST /api/auth/login` - Public
- `POST /api/auth/register` - `Auth:Create`
- `POST /api/auth/refresh` - Authenticated users

## Permission Usage Patterns

### Route Guards

```typescript
// Single permission
{
  path: 'users',
  canActivate: [AuthGuard, PermissionGuard],
  data: { permission: 'Users:List' }
}

// Multiple permissions (any)
{
  path: 'settings',
  canActivate: [AuthGuard, PermissionGuard],
  data: { 
    permission: ['Settings:Company:Read', 'Settings:User:Read'],
    permissionMode: 'any'
  }
}

// Multiple permissions (all)
{
  path: 'admin',
  canActivate: [AuthGuard, PermissionGuard],
  data: { 
    permission: ['Users:Update', 'Roles:Update'],
    permissionMode: 'all'
  }
}
```

### Component Directives

```html
<!-- Single permission -->
<button *appHasPermission="'Users:Create'" (click)="createUser()">
  Create User
</button>

<!-- Multiple permissions (any) -->
<div *appHasPermission="['Users:Update', 'Users:Delete']" appHasPermissionMode="any">
  User Actions
</div>

<!-- Multiple permissions (all) -->
<div *appHasPermission="['Users:Update', 'Users:Delete']" appHasPermissionMode="all">
  Full Access
</div>
```

### Data Table Actions

```typescript
actions: DataTableAction<User>[] = [
  {
    label: 'Edit',
    icon: 'edit',
    action: (row) => this.editUser(row),
    condition: () => this.permissionService.hasPermission('Users:Update')
  },
  {
    label: 'Delete',
    icon: 'trash',
    action: (row) => this.deleteUser(row),
    condition: (row) => !row.isSystemRole && this.permissionService.hasPermission('Users:Delete')
  }
];
```

### Service Methods

```typescript
// Check single permission
if (this.permissionService.hasPermission('Users:Update')) {
  // Allow action
}

// Check any permission
if (this.permissionService.hasAnyPermission(['Users:Update', 'Users:Delete'])) {
  // Allow action
}

// Check all permissions
if (this.permissionService.hasAllPermissions(['Users:Update', 'Users:Delete'])) {
  // Allow action
}
```

## Permission Hierarchy

Some permissions may have implicit relationships:

1. **List implies Read**: If a user has `Users:List`, they typically can view individual users (`Users:Read`)
2. **Update implies Read**: If a user has `Users:Update`, they can typically read user data
3. **Delete implies Read**: If a user has `Users:Delete`, they can typically read user data

However, these relationships should be explicitly granted in the role configuration for security.

## Best Practices

1. **Principle of Least Privilege**: Grant only the minimum permissions necessary
2. **Explicit Permissions**: Don't rely on implicit permission relationships
3. **Consistent Naming**: Use consistent permission naming across modules
4. **Documentation**: Document all permissions and their usage
5. **Testing**: Test permission checks in both frontend and backend
6. **Audit**: Regularly audit permissions to ensure they're still needed

## Common Permission Patterns

### CRUD Operations
- `{Module}:List` - List all records
- `{Module}:Read` - View single record
- `{Module}:Create` - Create new record
- `{Module}:Update` - Update existing record
- `{Module}:Delete` - Delete record

### Status Management
- `{Module}:Activate` - Activate record
- `{Module}:Deactivate` - Deactivate record

### Special Operations
- `{Module}:Assign` - Assign to another entity
- `{Module}:ChangePassword` - Change password (users)
- `{Module}:Export` - Export data
- `{Module}:Import` - Import data

## Permission Matrix

| Module | List | Read | Create | Update | Delete | Activate | Deactivate | Other |
|--------|------|------|--------|--------|-------|----------|------------|-------|
| Dashboard | - | ✓ | - | - | - | - | - | - |
| Users | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ✓ | ChangePassword |
| Roles | ✓ | ✓ | ✓ | ✓ | ✓ | - | - | Assign |
| Permissions | ✓ | ✓ | - | - | - | - | - | - |
| Settings | - | ✓ | - | ✓ | - | - | - | - |
| Theme | - | ✓ | - | ✓ | - | - | - | - |
| Auth | - | - | ✓ | - | - | - | - | - |

## Migration Notes

When adding new permissions:
1. Add permission to database seed data
2. Update this guide
3. Update API endpoint permissions
4. Update route guards
5. Update component directives
6. Update navigation service
7. Test thoroughly

## Troubleshooting

### Permission Not Working
1. Check if permission is granted to user's role
2. Verify permission name matches exactly (case-sensitive)
3. Check if user is authenticated
4. Verify permission is checked in both frontend and backend
5. Check browser console for errors
6. Verify token contains permission claims

### Permission Check Fails
1. Ensure `PermissionService` is injected
2. Verify `currentUser$` observable is subscribed
3. Check if permission is in the correct format
4. Verify permission exists in database
5. Check role-permission assignments

