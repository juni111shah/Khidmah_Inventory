# Database Seed Scripts

This directory contains SQL scripts to seed the database with initial data.

## Scripts Overview

1. **01_Companies.sql** - Inserts default company data
2. **02_Permissions.sql** - Inserts all system permissions
3. **03_Roles.sql** - Inserts default roles (Admin, Manager, User, etc.)
4. **04_Users.sql** - Inserts default users
5. **05_UserRoles.sql** - Assigns roles to users
6. **06_RolePermissions.sql** - Assigns permissions to roles
7. **07_UserCompanies.sql** - Links users to companies
8. **08_Categories.sql** - Inserts default product categories
9. **09_Brands.sql** - Inserts default product brands
10. **10_UnitsOfMeasure.sql** - Inserts default units of measure

## Execution Order

**IMPORTANT**: Execute scripts in numerical order (01, 02, 03, etc.) as they have dependencies.

### Option 1: Run Individual Scripts

Execute each script in order using SQL Server Management Studio or your preferred SQL client:

```sql
-- 1. Run 01_Companies.sql
-- 2. Run 02_Permissions.sql
-- 3. Run 03_Roles.sql
-- 4. Run 04_Users.sql
-- 5. Run 05_UserRoles.sql
-- 6. Run 06_RolePermissions.sql
-- 7. Run 07_UserCompanies.sql
-- 8. Run 08_Categories.sql
-- 9. Run 09_Brands.sql
-- 10. Run 10_UnitsOfMeasure.sql
```

### Option 2: Run All Scripts

You can also execute all scripts using the master script (if your SQL client supports it):

```sql
:r Database/Seed/01_Companies.sql
:r Database/Seed/02_Permissions.sql
:r Database/Seed/03_Roles.sql
:r Database/Seed/04_Users.sql
:r Database/Seed/05_UserRoles.sql
:r Database/Seed/06_RolePermissions.sql
:r Database/Seed/07_UserCompanies.sql
:r Database/Seed/08_Categories.sql
:r Database/Seed/09_Brands.sql
:r Database/Seed/10_UnitsOfMeasure.sql
```

## Configuration

Before running the scripts, you need to:

1. **Update CompanyId**: Replace `'00000000-0000-0000-0000-000000000001'` with your actual company ID in all scripts
2. **Update Password Hashes**: In `04_Users.sql`, replace the password hash with actual BCrypt hashes for your users
3. **Update GUIDs**: If you're using different GUIDs for roles/users, update them accordingly

## Default Users

After seeding, the following users will be created:

| Email | Username | Role | Password |
|-------|----------|------|----------|
| admin@khidmah.com | admin | Admin | Admin@123 (CHANGE THIS!) |
| manager@khidmah.com | manager | Manager | Admin@123 (CHANGE THIS!) |
| user@khidmah.com | user | User | Admin@123 (CHANGE THIS!) |
| warehouse@khidmah.com | warehouse | Warehouse Manager | Admin@123 (CHANGE THIS!) |
| sales@khidmah.com | sales | Sales Representative | Admin@123 (CHANGE THIS!) |

**⚠️ SECURITY WARNING**: Change all default passwords immediately after first login!

## Permissions

The seed scripts create comprehensive permissions for all modules:

- **Dashboard**: Read access
- **Auth**: User creation
- **Users**: Full CRUD + activate/deactivate/change password
- **Roles**: Full CRUD + assign
- **Permissions**: Read/List
- **Products**: Full CRUD + export
- **Categories**: Full CRUD
- **Brands**: Full CRUD
- **Warehouses**: Full CRUD
- **Inventory**: Stock levels, transactions, batches, serial numbers
- **Suppliers**: Full CRUD
- **Purchase Orders**: Full CRUD + approve
- **Customers**: Full CRUD
- **Sales Orders**: Full CRUD + approve
- **Companies**: Full CRUD
- **Settings**: All settings modules (Company, User, System, Notification, UI, Report, Layout, Theme)
- **Theme**: Read/Update
- **Reports**: Sales, Inventory, Purchase, Custom reports
- **System**: Admin, Audit, Backup
- **Reordering**: Suggestions and PO generation
- **Workflows**: Create, Start, Approve
- **Collaboration**: Activity feed and comments
- **AI**: Demand forecasting
- **Pricing**: Price suggestions
- **Documents**: Invoice and PO generation
- **Search**: Global search

## Role Permissions

### Admin Role
- **All permissions** - Full system access

### Manager Role
- Dashboard, Products, Categories, Brands, Warehouses
- Inventory management (stock levels, transactions, batches, serial numbers)
- Suppliers and Purchase Orders
- Customers and Sales Orders
- Reports (Sales, Inventory, Purchase)
- Settings (Company, User, UI)
- Theme management
- Reordering, Pricing, Documents, Search

### User Role
- Dashboard (read-only)
- Products, Categories, Brands, Warehouses (read-only)
- Inventory (read-only)
- Suppliers, Purchase Orders (read-only)
- Customers (read-only)
- Sales Orders (read and create)
- User settings (own profile)
- Theme (own theme)
- Search

### Warehouse Manager Role
- Dashboard
- Products, Categories, Brands (read-only)
- Warehouses (full access)
- Inventory management (full access)
- Inventory reports
- Search

### Sales Representative Role
- Dashboard
- Products, Categories (read-only)
- Customers (full access)
- Sales Orders (full access)
- Sales reports
- Invoice generation
- Search

### Purchase Manager Role
- Dashboard
- Products, Categories (read-only)
- Suppliers (full access)
- Purchase Orders (full access)
- Purchase reports
- Reordering suggestions
- PO generation
- Search

## Troubleshooting

### Error: Foreign Key Constraint
- Make sure scripts are executed in order
- Check that CompanyId matches in all scripts

### Error: Duplicate Key
- Scripts use `NEWID()` for most IDs, but some use fixed GUIDs
- If you get duplicate key errors, the data may already exist
- You can delete existing data or modify the GUIDs

### Password Hash Issues
- The default password hash in `04_Users.sql` is a placeholder
- Replace it with actual BCrypt hashes from your application
- Use: `BCrypt.Net.BCrypt.HashPassword("YourPassword")`

## Notes

- All timestamps use `GETUTCDATE()` for UTC time
- All records have `IsDeleted = 0` by default
- System roles (`IsSystemRole = 1`) cannot be updated or deleted
- The Admin user is assigned to the Admin role with all permissions

