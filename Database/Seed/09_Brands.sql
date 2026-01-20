-- =============================================
-- Brands Seed Data
-- =============================================
-- This script inserts default product brands

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'; -- Replace with your company ID
DECLARE @Now DATETIME2 = GETUTCDATE();

INSERT INTO Brands (Id, CompanyId, Name, Description, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Generic', 'Generic brand products', @Now, 0),
    (NEWID(), @CompanyId, 'Premium', 'Premium brand products', @Now, 0),
    (NEWID(), @CompanyId, 'Standard', 'Standard brand products', @Now, 0);

