-- =============================================
-- COMPLETE SEED DATA FOR KHIDMAH INVENTORY
-- =============================================
-- This script contains comprehensive INSERT statements for all entities
-- Execute this after running the base seed scripts (01-10)
-- 
-- IMPORTANT: 
-- 1. Replace @CompanyId with your actual company ID
-- 2. Replace password hashes with actual BCrypt hashes
-- 3. Adjust GUIDs if needed
-- =============================================

DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';
DECLARE @Now DATETIME2 = GETUTCDATE();
DECLARE @AdminUserId UNIQUEIDENTIFIER = '20000000-0000-0000-0000-000000000001';

-- =============================================
-- PRODUCTS
-- =============================================
PRINT 'Seeding Products...';

-- Get reference IDs
DECLARE @ElectronicsCategoryId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Categories WHERE CompanyId = @CompanyId AND Name = 'Electronics' AND IsDeleted = 0);
DECLARE @ClothingCategoryId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Categories WHERE Categories.CompanyId = @CompanyId AND Name = 'Clothing' AND IsDeleted = 0);
DECLARE @GenericBrandId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Brands WHERE CompanyId = @CompanyId AND Name = 'Generic' AND IsDeleted = 0);
DECLARE @PremiumBrandId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Brands WHERE CompanyId = @CompanyId AND Name = 'Premium' AND IsDeleted = 0);
DECLARE @PieceUnitId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM UnitOfMeasures WHERE CompanyId = @CompanyId AND Code = 'pcs' AND IsDeleted = 0);
DECLARE @KgUnitId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM UnitOfMeasures WHERE CompanyId = @CompanyId AND Code = 'kg' AND IsDeleted = 0);
DECLARE @LUnitId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM UnitOfMeasures WHERE CompanyId = @CompanyId AND Code = 'L' AND IsDeleted = 0);

INSERT INTO Products (
    Id, CompanyId, Name, Description, SKU, Barcode, CategoryId, BrandId, UnitOfMeasureId,
    PurchasePrice, SalePrice, CostPrice, MinStockLevel, MaxStockLevel, ReorderPoint,
    TrackQuantity, TrackBatch, TrackExpiry, IsActive, Weight, WeightUnit,
    Length, Width, Height, DimensionsUnit, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    -- Electronics Products
    (NEWID(), @CompanyId, 'Laptop Computer', 'High-performance laptop for business use', 'LAP-001', 'BCLAP00120250101', @ElectronicsCategoryId, @PremiumBrandId, @PieceUnitId,
     800.00, 1200.00, 750.00, 5, 50, 10, 1, 0, 0, 1, 2.5, 'kg', 35.5, 24.0, 2.5, 'cm', @Now, @AdminUserId, 0),
    
    (NEWID(), @CompanyId, 'Wireless Mouse', 'Ergonomic wireless mouse', 'MOU-001', 'BCMOU00120250101', @ElectronicsCategoryId, @GenericBrandId, @PieceUnitId,
     15.00, 25.00, 12.00, 20, 200, 50, 1, 0, 0, 1, 0.1, 'kg', 12.0, 6.0, 4.0, 'cm', @Now, @AdminUserId, 0),
    
    (NEWID(), @CompanyId, 'USB Keyboard', 'Mechanical USB keyboard', 'KEY-001', 'BCKEY00120250101', @ElectronicsCategoryId, @GenericBrandId, @PieceUnitId,
     30.00, 50.00, 25.00, 15, 150, 30, 1, 0, 0, 1, 0.8, 'kg', 45.0, 15.0, 3.0, 'cm', @Now, @AdminUserId, 0),
    
    (NEWID(), @CompanyId, 'Monitor 24 inch', 'Full HD 24-inch monitor', 'MON-001', 'BCMON00120250101', @ElectronicsCategoryId, @PremiumBrandId, @PieceUnitId,
     150.00, 250.00, 140.00, 10, 100, 20, 1, 0, 0, 1, 5.0, 'kg', 54.0, 34.0, 5.0, 'cm', @Now, @AdminUserId, 0),
    
    -- Clothing Products
    (NEWID(), @CompanyId, 'Cotton T-Shirt', '100% cotton t-shirt', 'TSH-001', 'BCTSH00120250101', @ClothingCategoryId, @GenericBrandId, @PieceUnitId,
     8.00, 15.00, 7.00, 50, 500, 100, 1, 0, 0, 1, 0.2, 'kg', NULL, NULL, NULL, NULL, @Now, @AdminUserId, 0),
    
    (NEWID(), @CompanyId, 'Denim Jeans', 'Classic blue denim jeans', 'JEA-001', 'BCJEA00120250101', @ClothingCategoryId, @PremiumBrandId, @PieceUnitId,
     25.00, 45.00, 22.00, 30, 300, 60, 1, 0, 0, 1, 0.6, 'kg', NULL, NULL, NULL, NULL, @Now, @AdminUserId, 0),
    
    (NEWID(), @CompanyId, 'Running Shoes', 'Comfortable running shoes', 'SHO-001', 'BCSHO00120250101', @ClothingCategoryId, @PremiumBrandId, @PieceUnitId,
     40.00, 80.00, 35.00, 20, 200, 40, 1, 0, 0, 1, 0.8, 'kg', NULL, NULL, NULL, NULL, @Now, @AdminUserId, 0);

PRINT '   ✓ Products seeded';

-- =============================================
-- WAREHOUSES
-- =============================================
PRINT 'Seeding Warehouses...';

DECLARE @MainWarehouseId UNIQUEIDENTIFIER = NEWID();
DECLARE @SecondaryWarehouseId UNIQUEIDENTIFIER = NEWID();

INSERT INTO Warehouses (
    Id, CompanyId, Name, Code, Description, Address, City, State, Country, 
    PostalCode, PhoneNumber, Email, IsDefault, IsActive, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@MainWarehouseId, @CompanyId, 'Main Warehouse', 'WH-001', 'Primary storage facility', 
     '100 Warehouse Street', 'New York', 'NY', 'United States', '10001', 
     '+1-555-1000', 'warehouse@khidmah.com', 1, 1, @Now, @AdminUserId, 0),
    
    (@SecondaryWarehouseId, @CompanyId, 'Secondary Warehouse', 'WH-002', 'Secondary storage facility', 
     '200 Storage Avenue', 'Los Angeles', 'CA', 'United States', '90001', 
     '+1-555-1001', 'warehouse2@khidmah.com', 0, 1, @Now, @AdminUserId, 0);

PRINT '   ✓ Warehouses seeded';

-- =============================================
-- SUPPLIERS
-- =============================================
PRINT 'Seeding Suppliers...';

DECLARE @Supplier1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Supplier2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Supplier3Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO Suppliers (
    Id, CompanyId, Name, Code, ContactPerson, Email, PhoneNumber, Address, City, State, 
    Country, PostalCode, TaxId, PaymentTerms, CreditLimit, Balance, IsActive, 
    CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@Supplier1Id, @CompanyId, 'Tech Supplies Inc.', 'SUP-001', 'John Smith', 
     'john@techsupplies.com', '+1-555-2000', '500 Tech Road', 'San Francisco', 'CA', 
     'United States', '94102', 'TAX-001', 'Net 30', 50000.00, 0.00, 1, @Now, @AdminUserId, 0),
    
    (@Supplier2Id, @CompanyId, 'Fashion Wholesale Co.', 'SUP-002', 'Sarah Johnson', 
     'sarah@fashionwholesale.com', '+1-555-2001', '600 Fashion Blvd', 'Miami', 'FL', 
     'United States', '33101', 'TAX-002', 'Net 15', 30000.00, 0.00, 1, @Now, @AdminUserId, 0),
    
    (@Supplier3Id, @CompanyId, 'General Merchandise Ltd.', 'SUP-003', 'Mike Brown', 
     'mike@genmerch.com', '+1-555-2002', '700 Commerce St', 'Chicago', 'IL', 
     'United States', '60601', 'TAX-003', 'Net 30', 40000.00, 0.00, 1, @Now, @AdminUserId, 0);

PRINT '   ✓ Suppliers seeded';

-- =============================================
-- CUSTOMERS
-- =============================================
PRINT 'Seeding Customers...';

DECLARE @Customer1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Customer2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @Customer3Id UNIQUEIDENTIFIER = NEWID();

INSERT INTO Customers (
    Id, CompanyId, Name, Code, ContactPerson, Email, PhoneNumber, Address, City, State, 
    Country, PostalCode, TaxId, PaymentTerms, CreditLimit, Balance, IsActive, 
    CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@Customer1Id, @CompanyId, 'ABC Retail Store', 'CUS-001', 'Robert Williams', 
     'robert@abcretail.com', '+1-555-3000', '800 Retail Avenue', 'New York', 'NY', 
     'United States', '10002', 'CUST-TAX-001', 'Net 30', 25000.00, 0.00, 1, @Now, @AdminUserId, 0),
    
    (@Customer2Id, @CompanyId, 'XYZ Electronics', 'CUS-002', 'Emily Davis', 
     'emily@xyzelectronics.com', '+1-555-3001', '900 Tech Plaza', 'Seattle', 'WA', 
     'United States', '98101', 'CUST-TAX-002', 'Net 15', 35000.00, 0.00, 1, @Now, @AdminUserId, 0),
    
    (@Customer3Id, @CompanyId, 'Fashion Boutique', 'CUS-003', 'Lisa Anderson', 
     'lisa@fashionboutique.com', '+1-555-3002', '100 Fashion Street', 'Los Angeles', 'CA', 
     'United States', '90002', 'CUST-TAX-003', 'Net 30', 20000.00, 0.00, 1, @Now, @AdminUserId, 0);

PRINT '   ✓ Customers seeded';

-- =============================================
-- PURCHASE ORDERS
-- =============================================
PRINT 'Seeding Purchase Orders...';

DECLARE @PO1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @PO2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @PO1Item1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @PO1Item2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @PO2Item1Id UNIQUEIDENTIFIER = NEWID();

-- Get Product IDs
DECLARE @LaptopProductId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Products WHERE CompanyId = @CompanyId AND SKU = 'LAP-001' AND IsDeleted = 0);
DECLARE @MouseProductId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Products WHERE CompanyId = @CompanyId AND SKU = 'MOU-001' AND IsDeleted = 0);
DECLARE @KeyboardProductId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Products WHERE CompanyId = @CompanyId AND SKU = 'KEY-001' AND IsDeleted = 0);

-- Purchase Order 1
INSERT INTO PurchaseOrders (
    Id, CompanyId, OrderNumber, SupplierId, OrderDate, ExpectedDeliveryDate,
    Status, SubTotal, TaxAmount, DiscountAmount, TotalAmount, Notes, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@PO1Id, @CompanyId, 'PO-2025-001', @Supplier1Id, 
     DATEADD(day, -10, @Now), DATEADD(day, 5, @Now), 'Draft', 830.00, 83.00, 0.00, 913.00, 
     'Initial stock order', DATEADD(day, -10, @Now), @AdminUserId, 0);

-- Purchase Order Items 1
INSERT INTO PurchaseOrderItems (
    Id, CompanyId, PurchaseOrderId, ProductId, Quantity, UnitPrice, DiscountPercent, 
    TaxPercent, LineTotal, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@PO1Item1Id, @CompanyId, @PO1Id, @LaptopProductId, 10, 800.00, 0.00, 10.00, 8800.00, DATEADD(day, -10, @Now), @AdminUserId, 0),
    (@PO1Item2Id, @CompanyId, @PO1Id, @MouseProductId, 50, 15.00, 0.00, 10.00, 825.00, DATEADD(day, -10, @Now), @AdminUserId, 0);

-- Purchase Order 2
INSERT INTO PurchaseOrders (
    Id, CompanyId, OrderNumber, SupplierId, OrderDate, ExpectedDeliveryDate,
    Status, SubTotal, TaxAmount, DiscountAmount, TotalAmount, Notes, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@PO2Id, @CompanyId, 'PO-2025-002', @Supplier1Id, 
     DATEADD(day, -5, @Now), DATEADD(day, 10, @Now), 'Sent', 30.00, 3.00, 0.00, 33.00, 
     'Keyboard restock', DATEADD(day, -5, @Now), @AdminUserId, 0);

-- Purchase Order Items 2
INSERT INTO PurchaseOrderItems (
    Id, CompanyId, PurchaseOrderId, ProductId, Quantity, UnitPrice, DiscountPercent, 
    TaxPercent, LineTotal, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@PO2Item1Id, @CompanyId, @PO2Id, @KeyboardProductId, 20, 30.00, 0.00, 10.00, 660.00, DATEADD(day, -5, @Now), @AdminUserId, 0);

PRINT '   ✓ Purchase Orders seeded';

-- =============================================
-- SALES ORDERS
-- =============================================
PRINT 'Seeding Sales Orders...';

DECLARE @SO1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @SO2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @SO1Item1Id UNIQUEIDENTIFIER = NEWID();
DECLARE @SO1Item2Id UNIQUEIDENTIFIER = NEWID();
DECLARE @SO2Item1Id UNIQUEIDENTIFIER = NEWID();

-- Sales Order 1
INSERT INTO SalesOrders (
    Id, CompanyId, OrderNumber, CustomerId, OrderDate, ExpectedDeliveryDate,
    Status, SubTotal, TaxAmount, DiscountAmount, TotalAmount, Notes, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@SO1Id, @CompanyId, 'SO-2025-001', @Customer1Id, 
     DATEADD(day, -7, @Now), DATEADD(day, 2, @Now), 'Delivered', 1225.00, 122.50, 0.00, 1347.50, 
     'Customer order', DATEADD(day, -7, @Now), @AdminUserId, 0);

-- Sales Order Items 1
INSERT INTO SalesOrderItems (
    Id, CompanyId, SalesOrderId, ProductId, Quantity, UnitPrice, DiscountPercent, 
    TaxPercent, LineTotal, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@SO1Item1Id, @CompanyId, @SO1Id, @LaptopProductId, 1, 1200.00, 0.00, 10.00, 1320.00, DATEADD(day, -7, @Now), @AdminUserId, 0),
    (@SO1Item2Id, @CompanyId, @SO1Id, @MouseProductId, 1, 25.00, 0.00, 10.00, 27.50, DATEADD(day, -7, @Now), @AdminUserId, 0);

-- Sales Order 2
INSERT INTO SalesOrders (
    Id, CompanyId, OrderNumber, CustomerId, OrderDate, ExpectedDeliveryDate,
    Status, SubTotal, TaxAmount, DiscountAmount, TotalAmount, Notes, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@SO2Id, @CompanyId, 'SO-2025-002', @Customer2Id, 
     DATEADD(day, -3, @Now), DATEADD(day, 5, @Now), 'Confirmed', 50.00, 5.00, 0.00, 55.00, 
     'Keyboard order', DATEADD(day, -3, @Now), @AdminUserId, 0);

-- Sales Order Items 2
INSERT INTO SalesOrderItems (
    Id, CompanyId, SalesOrderId, ProductId, Quantity, UnitPrice, DiscountPercent, 
    TaxPercent, LineTotal, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    (@SO2Item1Id, @CompanyId, @SO2Id, @KeyboardProductId, 1, 50.00, 0.00, 10.00, 55.00, DATEADD(day, -3, @Now), @AdminUserId, 0);

PRINT '   ✓ Sales Orders seeded';

-- =============================================
-- STOCK LEVELS
-- =============================================
PRINT 'Seeding Stock Levels...';

-- Get all product IDs
DECLARE @TShirtProductId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Products WHERE CompanyId = @CompanyId AND SKU = 'TSH-001' AND IsDeleted = 0);
DECLARE @JeansProductId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Products WHERE CompanyId = @CompanyId AND SKU = 'JEA-001' AND IsDeleted = 0);
DECLARE @ShoesProductId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Products WHERE CompanyId = @CompanyId AND SKU = 'SHO-001' AND IsDeleted = 0);
DECLARE @MonitorProductId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Products WHERE CompanyId = @CompanyId AND SKU = 'MON-001' AND IsDeleted = 0);

INSERT INTO StockLevels (
    Id, CompanyId, ProductId, WarehouseId, Quantity, ReservedQuantity, AverageCost,
    LastUpdated, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    -- Main Warehouse Stock
    (NEWID(), @CompanyId, @LaptopProductId, @MainWarehouseId, 8, 0, 800.00, @Now, @Now, @AdminUserId, 0),
    (NEWID(), @CompanyId, @MouseProductId, @MainWarehouseId, 49, 0, 15.00, @Now, @Now, @AdminUserId, 0),
    (NEWID(), @CompanyId, @KeyboardProductId, @MainWarehouseId, 19, 0, 30.00, @Now, @Now, @AdminUserId, 0),
    (NEWID(), @CompanyId, @MonitorProductId, @MainWarehouseId, 15, 0, 150.00, @Now, @Now, @AdminUserId, 0),
    (NEWID(), @CompanyId, @TShirtProductId, @MainWarehouseId, 100, 0, 8.00, @Now, @Now, @AdminUserId, 0),
    (NEWID(), @CompanyId, @JeansProductId, @MainWarehouseId, 50, 0, 25.00, @Now, @Now, @AdminUserId, 0),
    (NEWID(), @CompanyId, @ShoesProductId, @MainWarehouseId, 30, 0, 40.00, @Now, @Now, @AdminUserId, 0),
    
    -- Secondary Warehouse Stock
    (NEWID(), @CompanyId, @LaptopProductId, @SecondaryWarehouseId, 5, 0, 800.00, @Now, @Now, @AdminUserId, 0),
    (NEWID(), @CompanyId, @MouseProductId, @SecondaryWarehouseId, 25, 0, 15.00, @Now, @Now, @AdminUserId, 0),
    (NEWID(), @CompanyId, @TShirtProductId, @SecondaryWarehouseId, 75, 0, 8.00, @Now, @Now, @AdminUserId, 0);

PRINT '   ✓ Stock Levels seeded';

-- =============================================
-- STOCK TRANSACTIONS
-- =============================================
PRINT 'Seeding Stock Transactions...';

INSERT INTO StockTransactions (
    Id, CompanyId, ProductId, WarehouseId, TransactionType, Quantity, UnitCost, TotalCost,
    ReferenceType, ReferenceId, ReferenceNumber, TransactionDate, Notes, BalanceAfter, CreatedAt, CreatedBy, IsDeleted
)
VALUES
    -- Purchase transactions (StockIn)
    (NEWID(), @CompanyId, @LaptopProductId, @MainWarehouseId, 'StockIn', 10, 800.00, 8000.00,
     'PO', @PO1Id, 'PO-2025-001', DATEADD(day, -10, @Now), 'Initial stock purchase', 10, DATEADD(day, -10, @Now), @AdminUserId, 0),
    
    (NEWID(), @CompanyId, @MouseProductId, @MainWarehouseId, 'StockIn', 50, 15.00, 750.00,
     'PO', @PO1Id, 'PO-2025-001', DATEADD(day, -10, @Now), 'Mouse stock purchase', 50, DATEADD(day, -10, @Now), @AdminUserId, 0),
    
    (NEWID(), @CompanyId, @KeyboardProductId, @MainWarehouseId, 'StockIn', 20, 30.00, 600.00,
     'PO', @PO2Id, 'PO-2025-002', DATEADD(day, -5, @Now), 'Keyboard restock', 20, DATEADD(day, -5, @Now), @AdminUserId, 0),
    
    -- Sales transactions (StockOut)
    (NEWID(), @CompanyId, @LaptopProductId, @MainWarehouseId, 'StockOut', -1, 1200.00, -1200.00,
     'SO', @SO1Id, 'SO-2025-001', DATEADD(day, -7, @Now), 'Laptop sale', 9, DATEADD(day, -7, @Now), @AdminUserId, 0),
    
    (NEWID(), @CompanyId, @MouseProductId, @MainWarehouseId, 'StockOut', -1, 25.00, -25.00,
     'SO', @SO1Id, 'SO-2025-001', DATEADD(day, -7, @Now), 'Mouse sale', 49, DATEADD(day, -7, @Now), @AdminUserId, 0),
    
    -- Adjustment transactions
    (NEWID(), @CompanyId, @MonitorProductId, @MainWarehouseId, 'Adjustment', 15, 150.00, 2250.00,
     'Manual', NULL, NULL, DATEADD(day, -15, @Now), 'Initial stock adjustment', 15, DATEADD(day, -15, @Now), @AdminUserId, 0),
    
    (NEWID(), @CompanyId, @TShirtProductId, @MainWarehouseId, 'Adjustment', 100, 8.00, 800.00,
     'Manual', NULL, NULL, DATEADD(day, -15, @Now), 'Initial stock adjustment', 100, DATEADD(day, -15, @Now), @AdminUserId, 0),
    
    (NEWID(), @CompanyId, @JeansProductId, @MainWarehouseId, 'Adjustment', 50, 25.00, 1250.00,
     'Manual', NULL, NULL, DATEADD(day, -15, @Now), 'Initial stock adjustment', 50, DATEADD(day, -15, @Now), @AdminUserId, 0),
    
    (NEWID(), @CompanyId, @ShoesProductId, @MainWarehouseId, 'Adjustment', 30, 40.00, 1200.00,
     'Manual', NULL, NULL, DATEADD(day, -15, @Now), 'Initial stock adjustment', 30, DATEADD(day, -15, @Now), @AdminUserId, 0);

PRINT '   ✓ Stock Transactions seeded';

-- =============================================
-- COMPLETION MESSAGE
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'Complete seed data inserted successfully!';
PRINT '========================================';
PRINT '';
PRINT 'Summary:';
PRINT '  - Products: 7 items';
PRINT '  - Warehouses: 2 locations';
PRINT '  - Suppliers: 3 companies';
PRINT '  - Customers: 3 companies';
PRINT '  - Purchase Orders: 2 orders';
PRINT '  - Sales Orders: 2 orders';
PRINT '  - Stock Levels: 10 records';
PRINT '  - Stock Transactions: 9 transactions';
PRINT '';
PRINT 'You can now test the inventory system with this sample data.';
PRINT '';

