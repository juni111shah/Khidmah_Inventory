-- =============================================
-- Units of Measure Seed Data
-- =============================================
-- This script inserts default units of measure

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001'; -- Replace with your company ID
DECLARE @Now DATETIME2 = GETUTCDATE();

INSERT INTO UnitOfMeasures (Id, CompanyId, Name, Code, Description, IsBaseUnit, CreatedAt, IsDeleted)
VALUES
    (NEWID(), @CompanyId, 'Piece', 'pcs', 'Individual pieces or units', 1, @Now, 0),
    (NEWID(), @CompanyId, 'Kilogram', 'kg', 'Weight in kilograms', 1, @Now, 0),
    (NEWID(), @CompanyId, 'Gram', 'g', 'Weight in grams', 1, @Now, 0),
    (NEWID(), @CompanyId, 'Liter', 'L', 'Volume in liters', 1, @Now, 0),
    (NEWID(), @CompanyId, 'Milliliter', 'ml', 'Volume in milliliters', 1, @Now, 0),
    (NEWID(), @CompanyId, 'Meter', 'm', 'Length in meters', 1, @Now, 0),
    (NEWID(), @CompanyId, 'Centimeter', 'cm', 'Length in centimeters', 1, @Now, 0),
    (NEWID(), @CompanyId, 'Box', 'box', 'Box unit', 1, @Now, 0),
    (NEWID(), @CompanyId, 'Pack', 'pack', 'Pack unit', 1, @Now, 0),
    (NEWID(), @CompanyId, 'Carton', 'carton', 'Carton unit', 1, @Now, 0);

