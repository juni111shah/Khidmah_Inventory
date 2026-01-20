-- =============================================
-- RolePermissions Seed Data
-- =============================================
-- This script assigns permissions to roles
-- Note: This assigns ALL permissions to Admin role, and selective permissions to other roles

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'; -- Replace with your company ID
DECLARE @Now DATETIME2 = GETUTCDATE();

-- Role IDs
DECLARE @AdminRoleId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000001';
DECLARE @ManagerRoleId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000002';
DECLARE @UserRoleId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000003';
DECLARE @WarehouseManagerRoleId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000004';
DECLARE @SalesRepRoleId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000005';
DECLARE @PurchaseManagerRoleId UNIQUEIDENTIFIER = '10000000-0000-0000-0000-000000000006';

-- =============================================
-- ADMIN ROLE - ALL PERMISSIONS
-- =============================================
-- Assign all permissions to Admin role
INSERT INTO RolePermissions (Id, CompanyId, RoleId, PermissionId, CreatedAt, IsDeleted)
SELECT 
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    @AdminRoleId, -- RoleId
    Id, -- PermissionId
    @Now, -- CreatedAt
    0 -- IsDeleted
FROM Permissions
WHERE CompanyId = @CompanyId AND IsDeleted = 0;

-- =============================================
-- MANAGER ROLE - MANAGEMENT PERMISSIONS
-- =============================================
-- Dashboard
INSERT INTO RolePermissions (Id, CompanyId, RoleId, PermissionId, CreatedAt, IsDeleted)
SELECT NEWID(), @CompanyId, @ManagerRoleId, Id, @Now, 0
FROM Permissions
WHERE CompanyId = @CompanyId AND IsDeleted = 0
AND Name IN (
    'Dashboard:Read',
    'Products:List', 'Products:Read', 'Products:Create', 'Products:Update', 'Products:Delete', 'Products:Export',
    'Categories:List', 'Categories:Read', 'Categories:Create', 'Categories:Update', 'Categories:Delete',
    'Brands:List', 'Brands:Read', 'Brands:Create', 'Brands:Update', 'Brands:Delete',
    'Warehouses:List', 'Warehouses:Read', 'Warehouses:Create', 'Warehouses:Update', 'Warehouses:Delete',
    'Inventory:StockLevel:List', 'Inventory:StockLevel:Read', 'Inventory:StockLevel:Update',
    'Inventory:StockTransaction:Create', 'Inventory:StockTransaction:List', 'Inventory:StockTransaction:Read',
    'Inventory:Batch:Create', 'Inventory:Batch:List', 'Inventory:Batch:Update',
    'Inventory:SerialNumber:Create', 'Inventory:SerialNumber:List',
    'Suppliers:List', 'Suppliers:Read', 'Suppliers:Create', 'Suppliers:Update', 'Suppliers:Delete',
    'PurchaseOrders:List', 'PurchaseOrders:Read', 'PurchaseOrders:Create', 'PurchaseOrders:Update', 'PurchaseOrders:Approve',
    'Customers:List', 'Customers:Read', 'Customers:Create', 'Customers:Update', 'Customers:Delete',
    'SalesOrders:List', 'SalesOrders:Read', 'SalesOrders:Create', 'SalesOrders:Update', 'SalesOrders:Approve',
    'Reports:Sales:Read', 'Reports:Inventory:Read', 'Reports:Purchase:Read',
    'Settings:Company:Read', 'Settings:User:Read', 'Settings:UI:Read',
    'Theme:Read', 'Theme:Update',
    'Reordering:Suggestions:Read', 'Reordering:GeneratePO:Create',
    'Pricing:Suggestions:Read',
    'Documents:Invoice:Generate', 'Documents:PurchaseOrder:Generate',
    'Search:Global:Read'
);

-- =============================================
-- USER ROLE - BASIC PERMISSIONS
-- =============================================
-- Basic read-only and limited create permissions
INSERT INTO RolePermissions (Id, CompanyId, RoleId, PermissionId, CreatedAt, IsDeleted)
SELECT NEWID(), @CompanyId, @UserRoleId, Id, @Now, 0
FROM Permissions
WHERE CompanyId = @CompanyId AND IsDeleted = 0
AND Name IN (
    'Dashboard:Read',
    'Products:List', 'Products:Read',
    'Categories:List', 'Categories:Read',
    'Brands:List', 'Brands:Read',
    'Warehouses:List', 'Warehouses:Read',
    'Inventory:StockLevel:List', 'Inventory:StockLevel:Read',
    'Inventory:StockTransaction:List', 'Inventory:StockTransaction:Read',
    'Suppliers:List', 'Suppliers:Read',
    'PurchaseOrders:List', 'PurchaseOrders:Read',
    'Customers:List', 'Customers:Read',
    'SalesOrders:List', 'SalesOrders:Read', 'SalesOrders:Create',
    'Settings:User:Read', 'Settings:User:Update',
    'Settings:UserProfile:Read', 'Settings:UserProfile:Update',
    'Settings:UI:Read', 'Settings:UI:Update',
    'Theme:Read', 'Theme:Update',
    'Search:Global:Read'
);

-- =============================================
-- WAREHOUSE MANAGER ROLE
-- =============================================
INSERT INTO RolePermissions (Id, CompanyId, RoleId, PermissionId, CreatedAt, IsDeleted)
SELECT NEWID(), @CompanyId, @WarehouseManagerRoleId, Id, @Now, 0
FROM Permissions
WHERE CompanyId = @CompanyId AND IsDeleted = 0
AND Name IN (
    'Dashboard:Read',
    'Products:List', 'Products:Read',
    'Categories:List', 'Categories:Read',
    'Brands:List', 'Brands:Read',
    'Warehouses:List', 'Warehouses:Read', 'Warehouses:Create', 'Warehouses:Update',
    'Inventory:StockLevel:List', 'Inventory:StockLevel:Read', 'Inventory:StockLevel:Update',
    'Inventory:StockTransaction:Create', 'Inventory:StockTransaction:List', 'Inventory:StockTransaction:Read',
    'Inventory:Batch:Create', 'Inventory:Batch:List', 'Inventory:Batch:Update',
    'Inventory:SerialNumber:Create', 'Inventory:SerialNumber:List',
    'Reports:Inventory:Read',
    'Settings:User:Read',
    'Search:Global:Read'
);

-- =============================================
-- SALES REPRESENTATIVE ROLE
-- =============================================
INSERT INTO RolePermissions (Id, CompanyId, RoleId, PermissionId, CreatedAt, IsDeleted)
SELECT NEWID(), @CompanyId, @SalesRepRoleId, Id, @Now, 0
FROM Permissions
WHERE CompanyId = @CompanyId AND IsDeleted = 0
AND Name IN (
    'Dashboard:Read',
    'Products:List', 'Products:Read',
    'Categories:List', 'Categories:Read',
    'Customers:List', 'Customers:Read', 'Customers:Create', 'Customers:Update',
    'SalesOrders:List', 'SalesOrders:Read', 'SalesOrders:Create', 'SalesOrders:Update',
    'Reports:Sales:Read',
    'Documents:Invoice:Generate',
    'Settings:User:Read',
    'Search:Global:Read'
);

-- =============================================
-- PURCHASE MANAGER ROLE
-- =============================================
INSERT INTO RolePermissions (Id, CompanyId, RoleId, PermissionId, CreatedAt, IsDeleted)
SELECT NEWID(), @CompanyId, @PurchaseManagerRoleId, Id, @Now, 0
FROM Permissions
WHERE CompanyId = @CompanyId AND IsDeleted = 0
AND Name IN (
    'Dashboard:Read',
    'Products:List', 'Products:Read',
    'Categories:List', 'Categories:Read',
    'Suppliers:List', 'Suppliers:Read', 'Suppliers:Create', 'Suppliers:Update', 'Suppliers:Delete',
    'PurchaseOrders:List', 'PurchaseOrders:Read', 'PurchaseOrders:Create', 'PurchaseOrders:Update', 'PurchaseOrders:Approve',
    'Reports:Purchase:Read',
    'Reordering:Suggestions:Read', 'Reordering:GeneratePO:Create',
    'Documents:PurchaseOrder:Generate',
    'Settings:User:Read',
    'Search:Global:Read'
);

