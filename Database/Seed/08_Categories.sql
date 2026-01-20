-- =============================================
-- Categories Seed Data
-- =============================================
-- This script inserts default product categories

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'; -- Replace with your company ID
DECLARE @Now DATETIME2 = GETUTCDATE();

-- Root Categories
INSERT INTO Categories (Id, CompanyId, Name, Description, ParentCategoryId, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Electronics', 'Electronic products and devices', NULL, @Now, 0),
    (NEWID(), @CompanyId, 'Clothing', 'Clothing and apparel', NULL, @Now, 0),
    (NEWID(), @CompanyId, 'Food & Beverages', 'Food and beverage products', NULL, @Now, 0),
    (NEWID(), @CompanyId, 'Office Supplies', 'Office and stationery supplies', NULL, @Now, 0),
    (NEWID(), @CompanyId, 'Home & Garden', 'Home and garden products', NULL, @Now, 0);

-- Sub-categories for Electronics
DECLARE @ElectronicsId UNIQUEIDENTIFIER = (SELECT Id FROM Categories WHERE CompanyId = @CompanyId AND Name = 'Electronics' AND IsDeleted = 0);
INSERT INTO Categories (Id, CompanyId, Name, Description, ParentCategoryId, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Computers', 'Desktop and laptop computers', @ElectronicsId, @Now, 0),
    (NEWID(), @CompanyId, 'Mobile Phones', 'Smartphones and mobile devices', @ElectronicsId, @Now, 0),
    (NEWID(), @CompanyId, 'Accessories', 'Electronic accessories', @ElectronicsId, @Now, 0);

-- Sub-categories for Clothing
DECLARE @ClothingId UNIQUEIDENTIFIER = (SELECT Id FROM Categories WHERE CompanyId = @CompanyId AND Name = 'Clothing' AND IsDeleted = 0);
INSERT INTO Categories (Id, CompanyId, Name, Description, ParentCategoryId, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Men''s Clothing', 'Men''s apparel', @ClothingId, @Now, 0),
    (NEWID(), @CompanyId, 'Women''s Clothing', 'Women''s apparel', @ClothingId, @Now, 0),
    (NEWID(), @CompanyId, 'Children''s Clothing', 'Children''s apparel', @ClothingId, @Now, 0);

