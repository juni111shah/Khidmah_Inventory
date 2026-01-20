-- =============================================
-- Permissions Seed Data
-- =============================================
-- This script inserts all system permissions
-- Note: Replace CompanyId with your actual company ID

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'; -- Replace with your company ID
DECLARE @Now DATETIME2 = GETUTCDATE();

-- Dashboard Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Dashboard:Read', 'Dashboard', 'Read', 'Access dashboard page', @Now, 0);

-- Auth Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Auth:Create', 'Auth', 'Create', 'Create/register new users', @Now, 0);

-- Users Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Users:List', 'Users', 'List', 'View list of users', @Now, 0),
    (NEWID(), @CompanyId, 'Users:Read', 'Users', 'Read', 'View individual user details', @Now, 0),
    (NEWID(), @CompanyId, 'Users:Create', 'Users', 'Create', 'Create new users', @Now, 0),
    (NEWID(), @CompanyId, 'Users:Update', 'Users', 'Update', 'Update user information', @Now, 0),
    (NEWID(), @CompanyId, 'Users:Delete', 'Users', 'Delete', 'Delete users', @Now, 0),
    (NEWID(), @CompanyId, 'Users:Activate', 'Users', 'Activate', 'Activate users', @Now, 0),
    (NEWID(), @CompanyId, 'Users:Deactivate', 'Users', 'Deactivate', 'Deactivate users', @Now, 0),
    (NEWID(), @CompanyId, 'Users:ChangePassword', 'Users', 'ChangePassword', 'Change user password', @Now, 0);

-- Roles Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Roles:List', 'Roles', 'List', 'View list of roles', @Now, 0),
    (NEWID(), @CompanyId, 'Roles:Read', 'Roles', 'Read', 'View individual role details', @Now, 0),
    (NEWID(), @CompanyId, 'Roles:Create', 'Roles', 'Create', 'Create new roles', @Now, 0),
    (NEWID(), @CompanyId, 'Roles:Update', 'Roles', 'Update', 'Update role information', @Now, 0),
    (NEWID(), @CompanyId, 'Roles:Delete', 'Roles', 'Delete', 'Delete roles', @Now, 0),
    (NEWID(), @CompanyId, 'Roles:Assign', 'Roles', 'Assign', 'Assign roles to users', @Now, 0);

-- Permissions Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Permissions:List', 'Permissions', 'List', 'List all permissions', @Now, 0),
    (NEWID(), @CompanyId, 'Permissions:Read', 'Permissions', 'Read', 'View permissions list', @Now, 0);

-- Products Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Products:List', 'Products', 'List', 'List products', @Now, 0),
    (NEWID(), @CompanyId, 'Products:Read', 'Products', 'Read', 'View product details', @Now, 0),
    (NEWID(), @CompanyId, 'Products:Create', 'Products', 'Create', 'Create new products', @Now, 0),
    (NEWID(), @CompanyId, 'Products:Update', 'Products', 'Update', 'Update products', @Now, 0),
    (NEWID(), @CompanyId, 'Products:Delete', 'Products', 'Delete', 'Delete products', @Now, 0),
    (NEWID(), @CompanyId, 'Products:Export', 'Products', 'Export', 'Export products', @Now, 0);

-- Categories Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Categories:List', 'Categories', 'List', 'List categories', @Now, 0),
    (NEWID(), @CompanyId, 'Categories:Read', 'Categories', 'Read', 'View category details', @Now, 0),
    (NEWID(), @CompanyId, 'Categories:Create', 'Categories', 'Create', 'Create new categories', @Now, 0),
    (NEWID(), @CompanyId, 'Categories:Update', 'Categories', 'Update', 'Update categories', @Now, 0),
    (NEWID(), @CompanyId, 'Categories:Delete', 'Categories', 'Delete', 'Delete categories', @Now, 0);

-- Brands Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Brands:List', 'Brands', 'List', 'List brands', @Now, 0),
    (NEWID(), @CompanyId, 'Brands:Read', 'Brands', 'Read', 'View brand details', @Now, 0),
    (NEWID(), @CompanyId, 'Brands:Create', 'Brands', 'Create', 'Create new brands', @Now, 0),
    (NEWID(), @CompanyId, 'Brands:Update', 'Brands', 'Update', 'Update brands', @Now, 0),
    (NEWID(), @CompanyId, 'Brands:Delete', 'Brands', 'Delete', 'Delete brands', @Now, 0);

-- Warehouses Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Warehouses:List', 'Warehouses', 'List', 'List warehouses', @Now, 0),
    (NEWID(), @CompanyId, 'Warehouses:Read', 'Warehouses', 'Read', 'View warehouse details', @Now, 0),
    (NEWID(), @CompanyId, 'Warehouses:Create', 'Warehouses', 'Create', 'Create new warehouses', @Now, 0),
    (NEWID(), @CompanyId, 'Warehouses:Update', 'Warehouses', 'Update', 'Update warehouses', @Now, 0),
    (NEWID(), @CompanyId, 'Warehouses:Delete', 'Warehouses', 'Delete', 'Delete warehouses', @Now, 0);

-- Inventory Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Inventory:StockLevel:List', 'Inventory', 'StockLevel:List', 'List stock levels', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:StockLevel:Read', 'Inventory', 'StockLevel:Read', 'View stock level details', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:StockLevel:Update', 'Inventory', 'StockLevel:Update', 'Update stock levels', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:StockTransaction:Create', 'Inventory', 'StockTransaction:Create', 'Create stock transactions', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:StockTransaction:List', 'Inventory', 'StockTransaction:List', 'List stock transactions', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:StockTransaction:Read', 'Inventory', 'StockTransaction:Read', 'View stock transaction details', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:Batch:Create', 'Inventory', 'Batch:Create', 'Create batches', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:Batch:List', 'Inventory', 'Batch:List', 'List batches', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:Batch:Update', 'Inventory', 'Batch:Update', 'Update batches', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:SerialNumber:Create', 'Inventory', 'SerialNumber:Create', 'Create serial numbers', @Now, 0),
    (NEWID(), @CompanyId, 'Inventory:SerialNumber:List', 'Inventory', 'SerialNumber:List', 'List serial numbers', @Now, 0);

-- Suppliers Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Suppliers:List', 'Suppliers', 'List', 'List suppliers', @Now, 0),
    (NEWID(), @CompanyId, 'Suppliers:Read', 'Suppliers', 'Read', 'View supplier details', @Now, 0),
    (NEWID(), @CompanyId, 'Suppliers:Create', 'Suppliers', 'Create', 'Create new suppliers', @Now, 0),
    (NEWID(), @CompanyId, 'Suppliers:Update', 'Suppliers', 'Update', 'Update suppliers', @Now, 0),
    (NEWID(), @CompanyId, 'Suppliers:Delete', 'Suppliers', 'Delete', 'Delete suppliers', @Now, 0);

-- Purchase Orders Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'PurchaseOrders:List', 'PurchaseOrders', 'List', 'List purchase orders', @Now, 0),
    (NEWID(), @CompanyId, 'PurchaseOrders:Read', 'PurchaseOrders', 'Read', 'View purchase order details', @Now, 0),
    (NEWID(), @CompanyId, 'PurchaseOrders:Create', 'PurchaseOrders', 'Create', 'Create new purchase orders', @Now, 0),
    (NEWID(), @CompanyId, 'PurchaseOrders:Update', 'PurchaseOrders', 'Update', 'Update purchase orders', @Now, 0),
    (NEWID(), @CompanyId, 'PurchaseOrders:Delete', 'PurchaseOrders', 'Delete', 'Delete purchase orders', @Now, 0),
    (NEWID(), @CompanyId, 'PurchaseOrders:Approve', 'PurchaseOrders', 'Approve', 'Approve purchase orders', @Now, 0);

-- Customers Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Customers:List', 'Customers', 'List', 'List customers', @Now, 0),
    (NEWID(), @CompanyId, 'Customers:Read', 'Customers', 'Read', 'View customer details', @Now, 0),
    (NEWID(), @CompanyId, 'Customers:Create', 'Customers', 'Create', 'Create new customers', @Now, 0),
    (NEWID(), @CompanyId, 'Customers:Update', 'Customers', 'Update', 'Update customers', @Now, 0),
    (NEWID(), @CompanyId, 'Customers:Delete', 'Customers', 'Delete', 'Delete customers', @Now, 0);

-- Sales Orders Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'SalesOrders:List', 'SalesOrders', 'List', 'List sales orders', @Now, 0),
    (NEWID(), @CompanyId, 'SalesOrders:Read', 'SalesOrders', 'Read', 'View sales order details', @Now, 0),
    (NEWID(), @CompanyId, 'SalesOrders:Create', 'SalesOrders', 'Create', 'Create new sales orders', @Now, 0),
    (NEWID(), @CompanyId, 'SalesOrders:Update', 'SalesOrders', 'Update', 'Update sales orders', @Now, 0),
    (NEWID(), @CompanyId, 'SalesOrders:Delete', 'SalesOrders', 'Delete', 'Delete sales orders', @Now, 0),
    (NEWID(), @CompanyId, 'SalesOrders:Approve', 'SalesOrders', 'Approve', 'Approve sales orders', @Now, 0);

-- Companies Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Companies:List', 'Companies', 'List', 'List companies', @Now, 0),
    (NEWID(), @CompanyId, 'Companies:Read', 'Companies', 'Read', 'View company details', @Now, 0),
    (NEWID(), @CompanyId, 'Companies:Create', 'Companies', 'Create', 'Create new companies', @Now, 0),
    (NEWID(), @CompanyId, 'Companies:Update', 'Companies', 'Update', 'Update companies', @Now, 0),
    (NEWID(), @CompanyId, 'Companies:Delete', 'Companies', 'Delete', 'Delete companies', @Now, 0);

-- Settings Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Settings:Company:Read', 'Settings', 'Company:Read', 'View company settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Company:Update', 'Settings', 'Company:Update', 'Update company settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:User:Read', 'Settings', 'User:Read', 'View user settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:User:Update', 'Settings', 'User:Update', 'Update user settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:UserProfile:Read', 'Settings', 'UserProfile:Read', 'View user profile settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:UserProfile:Update', 'Settings', 'UserProfile:Update', 'Update user profile settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:System:Read', 'Settings', 'System:Read', 'View system settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:System:Update', 'Settings', 'System:Update', 'Update system settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Notification:Read', 'Settings', 'Notification:Read', 'View notification settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Notification:Update', 'Settings', 'Notification:Update', 'Update notification settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Notifications:Read', 'Settings', 'Notifications:Read', 'View notification settings (alternative)', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Notifications:Update', 'Settings', 'Notifications:Update', 'Update notification settings (alternative)', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:UI:Read', 'Settings', 'UI:Read', 'View UI settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:UI:Update', 'Settings', 'UI:Update', 'Update UI settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Report:Read', 'Settings', 'Report:Read', 'View report settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Report:Update', 'Settings', 'Report:Update', 'Update report settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Reports:Read', 'Settings', 'Reports:Read', 'View report settings (alternative)', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Reports:Update', 'Settings', 'Reports:Update', 'Update report settings (alternative)', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Layout:Read', 'Settings', 'Layout:Read', 'View layout settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Layout:Update', 'Settings', 'Layout:Update', 'Update layout settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Theme:Read', 'Settings', 'Theme:Read', 'View theme settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:Theme:Update', 'Settings', 'Theme:Update', 'Update theme settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:UIComponents:Read', 'Settings', 'UIComponents:Read', 'View UI component settings', @Now, 0),
    (NEWID(), @CompanyId, 'Settings:UIComponents:Update', 'Settings', 'UIComponents:Update', 'Update UI component settings', @Now, 0);

-- Theme Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Theme:Read', 'Theme', 'Read', 'View theme settings', @Now, 0),
    (NEWID(), @CompanyId, 'Theme:Update', 'Theme', 'Update', 'Update theme settings', @Now, 0);

-- Reports Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Reports:Sales:Read', 'Reports', 'Sales:Read', 'View sales reports', @Now, 0),
    (NEWID(), @CompanyId, 'Reports:Inventory:Read', 'Reports', 'Inventory:Read', 'View inventory reports', @Now, 0),
    (NEWID(), @CompanyId, 'Reports:Purchase:Read', 'Reports', 'Purchase:Read', 'View purchase reports', @Now, 0),
    (NEWID(), @CompanyId, 'Reports:Custom:Read', 'Reports', 'Custom:Read', 'View custom reports', @Now, 0),
    (NEWID(), @CompanyId, 'Reports:Custom:Create', 'Reports', 'Custom:Create', 'Create custom reports', @Now, 0),
    (NEWID(), @CompanyId, 'Reports:Custom:Execute', 'Reports', 'Custom:Execute', 'Execute custom reports', @Now, 0);

-- System Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'System:Admin', 'System', 'Admin', 'System administration', @Now, 0),
    (NEWID(), @CompanyId, 'System:Audit', 'System', 'Audit', 'View audit logs', @Now, 0),
    (NEWID(), @CompanyId, 'System:Backup', 'System', 'Backup', 'Backup/restore data', @Now, 0);

-- Reordering Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Reordering:Suggestions:Read', 'Reordering', 'Suggestions:Read', 'View reorder suggestions', @Now, 0),
    (NEWID(), @CompanyId, 'Reordering:GeneratePO:Create', 'Reordering', 'GeneratePO:Create', 'Generate purchase orders from suggestions', @Now, 0);

-- Workflows Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Workflows:Create', 'Workflows', 'Create', 'Create workflows', @Now, 0),
    (NEWID(), @CompanyId, 'Workflows:Start', 'Workflows', 'Start', 'Start workflow instances', @Now, 0),
    (NEWID(), @CompanyId, 'Workflows:Approve', 'Workflows', 'Approve', 'Approve workflow steps', @Now, 0);

-- Collaboration Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Collaboration:ActivityFeed:Read', 'Collaboration', 'ActivityFeed:Read', 'View activity feed', @Now, 0),
    (NEWID(), @CompanyId, 'Collaboration:Comments:Read', 'Collaboration', 'Comments:Read', 'View comments', @Now, 0),
    (NEWID(), @CompanyId, 'Collaboration:Comments:Create', 'Collaboration', 'Comments:Create', 'Create comments', @Now, 0);

-- AI Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'AI:DemandForecast:Read', 'AI', 'DemandForecast:Read', 'View demand forecasts', @Now, 0);

-- Pricing Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Pricing:Suggestions:Read', 'Pricing', 'Suggestions:Read', 'View price suggestions', @Now, 0);

-- Documents Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Documents:Invoice:Generate', 'Documents', 'Invoice:Generate', 'Generate invoice PDFs', @Now, 0),
    (NEWID(), @CompanyId, 'Documents:PurchaseOrder:Generate', 'Documents', 'PurchaseOrder:Generate', 'Generate purchase order PDFs', @Now, 0);

-- Search Permissions
INSERT INTO Permissions (Id, CompanyId, Name, Module, Action, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Search:Global:Read', 'Search', 'Global:Read', 'Perform global search', @Now, 0);

