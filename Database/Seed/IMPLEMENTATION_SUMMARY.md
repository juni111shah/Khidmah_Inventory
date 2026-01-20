# Database Seed Scripts & Dynamic Sidebar Implementation Summary

## Overview

This implementation provides:
1. **Complete SQL seed scripts** for all necessary database tables
2. **Dynamic sidebar** that loads menu items based on user permissions

## SQL Seed Scripts

### Location
All seed scripts are located in `Database/Seed/` directory.

### Scripts Created

1. **01_Companies.sql** - Default company data
2. **02_Permissions.sql** - All system permissions (100+ permissions)
3. **03_Roles.sql** - Default roles (Admin, Manager, User, Warehouse Manager, Sales Rep, Purchase Manager)
4. **04_Users.sql** - Default users with password hashes
5. **05_UserRoles.sql** - User-Role assignments
6. **06_RolePermissions.sql** - Role-Permission assignments (comprehensive permission mapping)
7. **07_UserCompanies.sql** - User-Company relationships
8. **08_Categories.sql** - Default product categories
9. **09_Brands.sql** - Default product brands
10. **10_UnitsOfMeasure.sql** - Default units of measure

### Execution

Execute scripts in numerical order (01 through 10) using SQL Server Management Studio or your preferred SQL client.

**Important Notes:**
- Replace `CompanyId` (`'00000000-0000-0000-0000-000000000001'`) with your actual company ID
- Replace password hashes in `04_Users.sql` with actual BCrypt hashes
- Default password for all users: `Admin@123` (CHANGE IMMEDIATELY!)

### Default Users

| Email | Username | Role | Permissions |
|-------|----------|------|-------------|
| admin@khidmah.com | admin | Admin | All permissions |
| manager@khidmah.com | manager | Manager | Management permissions |
| user@khidmah.com | user | User | Basic read permissions |
| warehouse@khidmah.com | warehouse | Warehouse Manager | Warehouse & inventory permissions |
| sales@khidmah.com | sales | Sales Representative | Sales & customer permissions |

## Dynamic Sidebar Implementation

### Changes Made

#### 1. **app.component.ts**
- Removed hardcoded `menuItems` array
- Added `NavigationService` injection
- Implemented `loadMenuItems()` method that subscribes to `NavigationService.getMenuItems()`
- Converts `NavigationItem[]` to `MenuItem[]` format
- Menu items now dynamically update based on user permissions

#### 2. **sidebar.component.ts**
- Updated to handle both scenarios:
  - When menu items are provided from parent (already filtered) - uses them directly
  - When no menu items provided - subscribes to `NavigationService` for dynamic filtering
- Maintains backward compatibility

### How It Works

1. **User logs in** → JWT token contains user permissions
2. **PermissionService** extracts permissions from token
3. **NavigationService** filters menu items based on permissions
4. **AppComponent** subscribes to filtered menu items
5. **Sidebar** displays only menu items user has permission to access

### Permission-Based Filtering

The sidebar automatically:
- Shows/hides menu items based on `permission` property
- Supports single permission: `permission: 'Users:List'`
- Supports multiple permissions: `permission: ['Users:List', 'Users:Read']`
- Supports permission modes:
  - `permissionMode: 'any'` - Show if user has ANY permission
  - `permissionMode: 'all'` - Show if user has ALL permissions
- Filters child menu items recursively
- Hides parent items if all children are filtered out

### Menu Structure

The navigation service defines menu items with permissions:

```typescript
{
  label: 'Users',
  icon: 'users',
  route: '/users',
  permission: 'Users:List'  // Only shows if user has this permission
}
```

For parent items with children:

```typescript
{
  label: 'Inventory',
  icon: 'inventory',
  permission: ['Inventory:StockLevel:List', 'Inventory:StockTransaction:Create'],
  permissionMode: 'any',  // Show if user has ANY of these permissions
  children: [
    {
      label: 'Stock Levels',
      route: '/inventory/stock-levels',
      permission: 'Inventory:StockLevel:List'
    },
    {
      label: 'Transfer Stock',
      route: '/inventory/transfer',
      permission: 'Inventory:StockTransaction:Create'
    }
  ]
}
```

## Testing

### Test Dynamic Sidebar

1. **Login as Admin** - Should see all menu items
2. **Login as User** - Should see limited menu items (Dashboard, Products read-only, etc.)
3. **Login as Warehouse Manager** - Should see warehouse and inventory items
4. **Login as Sales Rep** - Should see sales and customer items

### Test Permission Changes

1. **Change user role** - Sidebar should update automatically
2. **Remove permission from role** - Menu item should disappear
3. **Add permission to role** - Menu item should appear

## Security Notes

⚠️ **IMPORTANT SECURITY WARNINGS:**

1. **Change Default Passwords**: All default users have password `Admin@123` - CHANGE IMMEDIATELY!
2. **Password Hashes**: Replace placeholder hashes in `04_Users.sql` with actual BCrypt hashes
3. **Company IDs**: Use unique GUIDs for production
4. **System Roles**: Admin, Manager, and User roles are marked as system roles and cannot be deleted

## Permissions Coverage

The seed scripts create permissions for:

- ✅ Dashboard
- ✅ Authentication
- ✅ Users (CRUD + Activate/Deactivate/ChangePassword)
- ✅ Roles (CRUD + Assign)
- ✅ Permissions (Read/List)
- ✅ Products (CRUD + Export)
- ✅ Categories (CRUD)
- ✅ Brands (CRUD)
- ✅ Warehouses (CRUD)
- ✅ Inventory (Stock Levels, Transactions, Batches, Serial Numbers)
- ✅ Suppliers (CRUD)
- ✅ Purchase Orders (CRUD + Approve)
- ✅ Customers (CRUD)
- ✅ Sales Orders (CRUD + Approve)
- ✅ Companies (CRUD)
- ✅ Settings (All modules: Company, User, System, Notification, UI, Report, Layout, Theme)
- ✅ Theme (Read/Update)
- ✅ Reports (Sales, Inventory, Purchase, Custom)
- ✅ System (Admin, Audit, Backup)
- ✅ Reordering (Suggestions, PO Generation)
- ✅ Workflows (Create, Start, Approve)
- ✅ Collaboration (Activity Feed, Comments)
- ✅ AI (Demand Forecast)
- ✅ Pricing (Suggestions)
- ✅ Documents (Invoice, PO Generation)
- ✅ Search (Global)

## Next Steps

1. **Run Seed Scripts**: Execute all SQL scripts in order
2. **Update Passwords**: Change all default user passwords
3. **Test Login**: Login with different users and verify sidebar shows correct items
4. **Customize**: Add/remove permissions as needed for your use case
5. **Production**: Review and adjust seed data for production environment

## Troubleshooting

### Sidebar Not Showing Items
- Check if user is logged in
- Verify JWT token contains permissions
- Check browser console for errors
- Verify `NavigationService` is working
- Check if permissions are assigned to user's role

### SQL Script Errors
- Ensure scripts run in order (01-10)
- Check CompanyId matches in all scripts
- Verify GUIDs are unique
- Check for foreign key constraints

### Permission Not Working
- Verify permission exists in database
- Check permission is assigned to user's role
- Verify permission name matches exactly (case-sensitive)
- Check JWT token contains the permission

