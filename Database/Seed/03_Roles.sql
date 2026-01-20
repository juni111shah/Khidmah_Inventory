-- =============================================
-- Roles Seed Data
-- =============================================
-- This script inserts default roles
-- Note: Replace CompanyId with your actual company ID

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'; -- Replace with your company ID
DECLARE @Now DATETIME2 = GETUTCDATE();

-- Admin Role (System Role - Full Access)
INSERT INTO Roles (Id, CompanyId, Name, Description, IsSystemRole, CreatedAt, IsDeleted)
VALUES (
    '10000000-0000-0000-0000-000000000001', -- Id
    @CompanyId, -- CompanyId
    'Admin', -- Name
    'System administrator with full access to all features', -- Description
    1, -- IsSystemRole
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Manager Role (System Role - Management Access)
INSERT INTO Roles (Id, CompanyId, Name, Description, IsSystemRole, CreatedAt, IsDeleted)
VALUES (
    '10000000-0000-0000-0000-000000000002', -- Id
    @CompanyId, -- CompanyId
    'Manager', -- Name
    'Manager with access to manage products, inventory, orders, and reports', -- Description
    1, -- IsSystemRole
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- User Role (System Role - Basic Access)
INSERT INTO Roles (Id, CompanyId, Name, Description, IsSystemRole, CreatedAt, IsDeleted)
VALUES (
    '10000000-0000-0000-0000-000000000003', -- Id
    @CompanyId, -- CompanyId
    'User', -- Name
    'Standard user with limited access based on assigned permissions', -- Description
    1, -- IsSystemRole
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Warehouse Manager Role (Custom Role)
INSERT INTO Roles (Id, CompanyId, Name, Description, IsSystemRole, CreatedAt, IsDeleted)
VALUES (
    '10000000-0000-0000-0000-000000000004', -- Id
    @CompanyId, -- CompanyId
    'Warehouse Manager', -- Name
    'Manages warehouses, inventory, and stock transactions', -- Description
    0, -- IsSystemRole
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Sales Representative Role (Custom Role)
INSERT INTO Roles (Id, CompanyId, Name, Description, IsSystemRole, CreatedAt, IsDeleted)
VALUES (
    '10000000-0000-0000-0000-000000000005', -- Id
    @CompanyId, -- CompanyId
    'Sales Representative', -- Name
    'Manages customers and sales orders', -- Description
    0, -- IsSystemRole
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Purchase Manager Role (Custom Role)
INSERT INTO Roles (Id, CompanyId, Name, Description, IsSystemRole, CreatedAt, IsDeleted)
VALUES (
    '10000000-0000-0000-0000-000000000006', -- Id
    @CompanyId, -- CompanyId
    'Purchase Manager', -- Name
    'Manages suppliers and purchase orders', -- Description
    0, -- IsSystemRole
    @Now, -- CreatedAt
    0 -- IsDeleted
);

