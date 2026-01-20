-- =============================================
-- Users Seed Data
-- =============================================
-- This script inserts default users
-- Note: Replace CompanyId with your actual company ID
-- IMPORTANT: Replace PasswordHash with actual hashed passwords using your password hashing algorithm

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'; -- Replace with your company ID
DECLARE @Now DATETIME2 = GETUTCDATE();

-- Default password hash for "Admin@123" (BCrypt hash)
-- You should replace this with your actual password hash
-- Use BCrypt.Net or similar to generate: BCrypt.Net.BCrypt.HashPassword("Admin@123")
DECLARE @DefaultPasswordHash NVARCHAR(255) = '$2a$11$KIXLQZxJfNt5Y5Y5Y5Y5YOu5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y'; -- Replace with actual hash

-- Admin User
INSERT INTO Users (
    Id,
    CompanyId,
    Email,
    UserName,
    PasswordHash,
    FirstName,
    LastName,
    PhoneNumber,
    IsActive,
    EmailConfirmed,
    CreatedAt,
    IsDeleted
)
VALUES (
    '20000000-0000-0000-0000-000000000001', -- Id
    @CompanyId, -- CompanyId
    'admin@khidmah.com', -- Email
    'admin', -- UserName
    @DefaultPasswordHash, -- PasswordHash (REPLACE WITH ACTUAL HASH)
    'System', -- FirstName
    'Administrator', -- LastName
    '+1-555-0100', -- PhoneNumber
    1, -- IsActive
    1, -- EmailConfirmed
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Manager User
INSERT INTO Users (
    Id,
    CompanyId,
    Email,
    UserName,
    PasswordHash,
    FirstName,
    LastName,
    PhoneNumber,
    IsActive,
    EmailConfirmed,
    CreatedAt,
    IsDeleted
)
VALUES (
    '20000000-0000-0000-0000-000000000002', -- Id
    @CompanyId, -- CompanyId
    'manager@khidmah.com', -- Email
    'manager', -- UserName
    @DefaultPasswordHash, -- PasswordHash (REPLACE WITH ACTUAL HASH)
    'John', -- FirstName
    'Manager', -- LastName
    '+1-555-0101', -- PhoneNumber
    1, -- IsActive
    1, -- EmailConfirmed
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Standard User
INSERT INTO Users (
    Id,
    CompanyId,
    Email,
    UserName,
    PasswordHash,
    FirstName,
    LastName,
    PhoneNumber,
    IsActive,
    EmailConfirmed,
    CreatedAt,
    IsDeleted
)
VALUES (
    '20000000-0000-0000-0000-000000000003', -- Id
    @CompanyId, -- CompanyId
    'user@khidmah.com', -- Email
    'user', -- UserName
    @DefaultPasswordHash, -- PasswordHash (REPLACE WITH ACTUAL HASH)
    'Jane', -- FirstName
    'User', -- LastName
    '+1-555-0102', -- PhoneNumber
    1, -- IsActive
    1, -- EmailConfirmed
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Warehouse Manager User
INSERT INTO Users (
    Id,
    CompanyId,
    Email,
    UserName,
    PasswordHash,
    FirstName,
    LastName,
    PhoneNumber,
    IsActive,
    EmailConfirmed,
    CreatedAt,
    IsDeleted
)
VALUES (
    '20000000-0000-0000-0000-000000000004', -- Id
    @CompanyId, -- CompanyId
    'warehouse@khidmah.com', -- Email
    'warehouse', -- UserName
    @DefaultPasswordHash, -- PasswordHash (REPLACE WITH ACTUAL HASH)
    'Bob', -- FirstName
    'Warehouse', -- LastName
    '+1-555-0103', -- PhoneNumber
    1, -- IsActive
    1, -- EmailConfirmed
    @Now, -- CreatedAt
    0 -- IsDeleted
);

-- Sales Representative User
INSERT INTO Users (
    Id,
    CompanyId,
    Email,
    UserName,
    PasswordHash,
    FirstName,
    LastName,
    PhoneNumber,
    IsActive,
    EmailConfirmed,
    CreatedAt,
    IsDeleted
)
VALUES (
    '20000000-0000-0000-0000-000000000005', -- Id
    @CompanyId, -- CompanyId
    'sales@khidmah.com', -- Email
    'sales', -- UserName
    @DefaultPasswordHash, -- PasswordHash (REPLACE WITH ACTUAL HASH)
    'Alice', -- FirstName
    'Sales', -- LastName
    '+1-555-0104', -- PhoneNumber
    1, -- IsActive
    1, -- EmailConfirmed
    @Now, -- CreatedAt
    0 -- IsDeleted
);

