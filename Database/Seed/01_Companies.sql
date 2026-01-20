-- =============================================
-- Companies Seed Data
-- =============================================
-- This script inserts default company data
-- Note: Replace the GUIDs with your own if needed

-- Default Company
INSERT INTO Companies (
    Id,
    CompanyId,
    Name,
    LegalName,
    Email,
    PhoneNumber,
    Address,
    City,
    State,
    Country,
    PostalCode,
    Currency,
    TimeZone,
    IsActive,
    CreatedAt,
    UpdatedAt,
    CreatedBy,
    UpdatedBy,
    IsDeleted,
    DeletedAt,
    DeletedBy
)
VALUES (
    '00000000-0000-0000-0000-000000000001', -- Id (same as CompanyId for root company)
    '00000000-0000-0000-0000-000000000001', -- CompanyId
    'Khidmah Inventory Demo', -- Name
    'Khidmah Inventory Demo LLC', -- LegalName
    'admin@khidmah.com', -- Email
    '+1-555-0100', -- PhoneNumber
    '123 Business Street', -- Address
    'New York', -- City
    'NY', -- State
    'United States', -- Country
    '10001', -- PostalCode
    'USD', -- Currency
    'America/New_York', -- TimeZone
    1, -- IsActive
    GETUTCDATE(), -- CreatedAt
    NULL, -- UpdatedAt
    NULL, -- CreatedBy (system)
    NULL, -- UpdatedBy
    0, -- IsDeleted
    NULL, -- DeletedAt
    NULL -- DeletedBy
);

