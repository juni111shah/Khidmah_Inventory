-- =============================================
-- UserRoles Seed Data
-- =============================================
-- This script assigns roles to users
-- Note: Replace IDs with your actual User and Role IDs

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'; -- Replace with your company ID
DECLARE @Now DATETIME2 = GETUTCDATE();

-- Admin User -> Admin Role
INSERT INTO UserRoles (Id, CompanyId, UserId, RoleId, CreatedAt, IsDeleted)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000001', -- UserId (Admin User)
    '10000000-0000-0000-0000-000000000001', -- RoleId (Admin Role)
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Manager User -> Manager Role
INSERT INTO UserRoles (Id, CompanyId, UserId, RoleId, CreatedAt, IsDeleted)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000002', -- UserId (Manager User)
    '10000000-0000-0000-0000-000000000002', -- RoleId (Manager Role)
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Standard User -> User Role
INSERT INTO UserRoles (Id, CompanyId, UserId, RoleId, CreatedAt, IsDeleted)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000003', -- UserId (Standard User)
    '10000000-0000-0000-0000-000000000003', -- RoleId (User Role)
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Warehouse Manager User -> Warehouse Manager Role
INSERT INTO UserRoles (Id, CompanyId, UserId, RoleId, CreatedAt, IsDeleted)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000004', -- UserId (Warehouse Manager User)
    '10000000-0000-0000-0000-000000000004', -- RoleId (Warehouse Manager Role)
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Sales Representative User -> Sales Representative Role
INSERT INTO UserRoles (Id, CompanyId, UserId, RoleId, CreatedAt, IsDeleted)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000005', -- UserId (Sales Representative User)
    '10000000-0000-0000-0000-000000000005', -- RoleId (Sales Representative Role)
    @Now, -- CreatedAt
    0 -- IsDeleted
);

