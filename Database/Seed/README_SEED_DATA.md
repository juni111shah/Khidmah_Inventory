# Complete SQL Seed Data Guide

This guide explains all the SQL INSERT queries available for seeding the Khidmah Inventory database.

## 📋 Seed Files Overview

### Base Seed Files (Run First)
1. **01_Companies.sql** - Creates the default company
2. **02_Permissions.sql** - Inserts all system permissions
3. **03_Roles.sql** - Creates default roles (Admin, Manager, User, etc.)
4. **04_Users.sql** - Creates default users
5. **05_UserRoles.sql** - Assigns roles to users
6. **06_RolePermissions.sql** - Assigns permissions to roles
7. **07_UserCompanies.sql** - Links users to companies
8. **08_Categories.sql** - Creates product categories
9. **09_Brands.sql** - Creates product brands
10. **10_UnitsOfMeasure.sql** - Creates units of measure

### Complete Seed Data (Run After Base Files)
11. **COMPLETE_SEED_DATA.sql** - Comprehensive data for:
    - Products (7 sample products)
    - Warehouses (2 locations)
    - Suppliers (3 companies)
    - Customers (3 companies)
    - Purchase Orders (2 orders with items)
    - Sales Orders (2 orders with items)
    - Stock Levels (10 records)
    - Stock Transactions (9 transactions)

## 🚀 Quick Start

### Option 1: Run All Seeds in Order
```sql
-- Execute each file in sequence:
-- 1. 01_Companies.sql
-- 2. 02_Permissions.sql
-- 3. 03_Roles.sql
-- 4. 04_Users.sql
-- 5. 05_UserRoles.sql
-- 6. 06_RolePermissions.sql
-- 7. 07_UserCompanies.sql
-- 8. 08_Categories.sql
-- 9. 09_Brands.sql
-- 10. 10_UnitsOfMeasure.sql
-- 11. COMPLETE_SEED_DATA.sql
```

### Option 2: Use the Master Script
```sql
-- Run 00_RunAllSeeds.sql (executes files 01-10)
-- Then run COMPLETE_SEED_DATA.sql
```

## 📊 Data Summary

### Users Created
- **Admin**: admin@khidmah.com / Admin@123
- **Manager**: manager@khidmah.com / Admin@123
- **User**: user@khidmah.com / Admin@123
- **Warehouse Manager**: warehouse@khidmah.com / Admin@123
- **Sales Rep**: sales@khidmah.com / Admin@123

### Products Created (7 items)
- Laptop Computer (Electronics)
- Wireless Mouse (Electronics)
- USB Keyboard (Electronics)
- Monitor 24 inch (Electronics)
- Cotton T-Shirt (Clothing)
- Denim Jeans (Clothing)
- Running Shoes (Clothing)

### Warehouses Created (2 locations)
- Main Warehouse (WH-001) - Default
- Secondary Warehouse (WH-002)

### Suppliers Created (3 companies)
- Tech Supplies Inc. (SUP-001)
- Fashion Wholesale Co. (SUP-002)
- General Merchandise Ltd. (SUP-003)

### Customers Created (3 companies)
- ABC Retail Store (CUS-001)
- XYZ Electronics (CUS-002)
- Fashion Boutique (CUS-003)

### Orders Created
- **Purchase Orders**: 2 orders with 3 line items
- **Sales Orders**: 2 orders with 3 line items

### Stock Data
- **Stock Levels**: 10 records across 2 warehouses
- **Stock Transactions**: 9 transactions (purchases, sales, adjustments)

## ⚙️ Configuration

### Before Running Scripts

1. **Update Company ID** (if needed):
   ```sql
   DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';
   ```

2. **Update Password Hashes**:
   - The scripts use placeholder password hashes
   - Generate actual BCrypt hashes for "Admin@123"
   - Update in `04_Users.sql`:
     ```sql
     DECLARE @DefaultPasswordHash NVARCHAR(255) = 'YOUR_BCRYPT_HASH_HERE';
     ```

3. **Verify GUIDs**:
   - All scripts use consistent GUIDs
   - If you change CompanyId, update it in all scripts

## 📝 SQL INSERT Statement Structure

All INSERT statements follow this pattern:

```sql
INSERT INTO TableName (
    Id,                    -- Unique identifier (GUID)
    CompanyId,             -- Multi-tenant company ID
    -- ... other columns ...
    CreatedAt,             -- Audit field
    CreatedBy,             -- Audit field
    IsDeleted              -- Soft delete flag
)
VALUES (
    NEWID(),               -- Generate new GUID
    @CompanyId,            -- Use declared company ID
    -- ... values ...
    GETUTCDATE(),          -- Current UTC time
    @AdminUserId,          -- Creator user ID
    0                      -- Not deleted
);
```

## 🔍 Key Tables and Their Data

### Products Table
- **Columns**: Name, SKU, Barcode, CategoryId, BrandId, UnitOfMeasureId, PurchasePrice, SalePrice, etc.
- **Sample Data**: 7 products across Electronics and Clothing categories

### Warehouses Table
- **Columns**: Name, Code, Address, City, State, Country, IsDefault, IsActive
- **Sample Data**: 2 warehouses with full address information

### Suppliers Table
- **Columns**: Name, Code, ContactPerson, Email, PhoneNumber, Address, CreditLimit, Balance
- **Sample Data**: 3 suppliers with contact and financial information

### Customers Table
- **Columns**: Name, Code, ContactPerson, Email, PhoneNumber, Address, CreditLimit, Balance
- **Sample Data**: 3 customers with contact and financial information

### PurchaseOrders Table
- **Columns**: OrderNumber, SupplierId, WarehouseId, OrderDate, Status, TotalAmount
- **Sample Data**: 2 purchase orders with different statuses

### SalesOrders Table
- **Columns**: OrderNumber, CustomerId, WarehouseId, OrderDate, Status, TotalAmount
- **Sample Data**: 2 sales orders (1 completed, 1 pending)

### StockLevels Table
- **Columns**: ProductId, WarehouseId, Quantity, ReservedQuantity, AvailableQuantity, ReorderLevel
- **Sample Data**: 10 stock level records across products and warehouses

### StockTransactions Table
- **Columns**: ProductId, WarehouseId, TransactionType, Quantity, UnitCost, ReferenceType, ReferenceId
- **Sample Data**: 9 transactions (purchases, sales, adjustments)

## 🔐 Security Notes

⚠️ **IMPORTANT**: 
- Default passwords are "Admin@123" for all users
- **Change passwords immediately after first login**
- Password hashes must be generated using your application's hashing algorithm (BCrypt)

## 📈 Testing the Data

After running all seed scripts, you can:

1. **Login** with any of the created users
2. **View Dashboard** - Should show summary cards with data
3. **Browse Products** - See 7 sample products
4. **View Warehouses** - See 2 warehouse locations
5. **Check Inventory** - View stock levels and transactions
6. **View Orders** - See purchase and sales orders
7. **Test Reports** - Generate reports with sample data

## 🛠️ Troubleshooting

### Common Issues

1. **Foreign Key Violations**:
   - Ensure scripts run in the correct order
   - Base seed files (01-10) must run before COMPLETE_SEED_DATA.sql

2. **Duplicate Key Errors**:
   - Check if data already exists
   - Use DELETE statements before INSERT if re-running

3. **Password Hash Issues**:
   - Generate hash using: `BCrypt.Net.BCrypt.HashPassword("Admin@123")`
   - Update in 04_Users.sql before running

4. **Company ID Mismatch**:
   - Ensure @CompanyId is the same in all scripts
   - Default: '00000000-0000-0000-0000-000000000001'

## 📚 Additional Resources

- See individual seed files for detailed comments
- Check `00_RunAllSeeds.sql` for execution order
- Review entity classes in `Khidmah_Inventory.Domain/Entities/` for field definitions

## ✅ Verification Checklist

After running all scripts, verify:

- [ ] Company created
- [ ] Permissions inserted (200+ permissions)
- [ ] Roles created (6 roles)
- [ ] Users created (5 users)
- [ ] User-Role assignments complete
- [ ] Role-Permission assignments complete
- [ ] User-Company links complete
- [ ] Categories created (8 categories)
- [ ] Brands created (3 brands)
- [ ] Units of measure created (10 units)
- [ ] Products created (7 products)
- [ ] Warehouses created (2 warehouses)
- [ ] Suppliers created (3 suppliers)
- [ ] Customers created (3 customers)
- [ ] Purchase orders created (2 orders)
- [ ] Sales orders created (2 orders)
- [ ] Stock levels created (10 records)
- [ ] Stock transactions created (9 transactions)

---

**Last Updated**: 2025-01-02
**Version**: 1.0

