-- =============================================
-- UserCompanies Seed Data
-- =============================================
-- This script links users to companies
-- Note: Replace IDs with your actual User and Company IDs

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'; -- Replace with your company ID
DECLARE @Now DATETIME2 = GETUTCDATE();

-- Admin User -> Company
INSERT INTO UserCompanies (Id, CompanyId, UserId, CreatedAt, IsDeleted, IsDefault, IsActive)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000001', -- UserId (Admin User)
    @Now, -- CreatedAt
    0, -- IsDeleted
    1, -- IsDefault
    1 -- IsActive
);

-- Manager User -> Company
INSERT INTO UserCompanies (Id, CompanyId, UserId, CreatedAt, IsDeleted, IsDefault, IsActive)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000002', -- UserId (Manager User)
    @Now, -- CreatedAt
    0, -- IsDeleted
    1, -- IsDefault
    1 -- IsActive
);

-- Standard User -> Company
INSERT INTO UserCompanies (Id, CompanyId, UserId, CreatedAt, IsDeleted, IsDefault, IsActive)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000003', -- UserId (Standard User)
    @Now, -- CreatedAt
    0, -- IsDeleted
    1, -- IsDefault
    1 -- IsActive
);

-- Warehouse Manager User -> Company
INSERT INTO UserCompanies (Id, CompanyId, UserId, CreatedAt, IsDeleted, IsDefault, IsActive)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000004', -- UserId (Warehouse Manager User)
    @Now, -- CreatedAt
    0, -- IsDeleted
    1, -- IsDefault
    1 -- IsActive
);

-- Sales Representative User -> Company
INSERT INTO UserCompanies (Id, CompanyId, UserId, CreatedAt, IsDeleted, IsDefault, IsActive)
VALUES (
    NEWID(), -- Id
    @CompanyId, -- CompanyId
    '20000000-0000-0000-0000-000000000005', -- UserId (Sales Representative User)
    @Now, -- CreatedAt
    0, -- IsDeleted
    1, -- IsDefault
    1 -- IsActive
);

