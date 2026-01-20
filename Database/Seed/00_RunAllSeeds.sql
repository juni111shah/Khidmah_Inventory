-- =============================================
-- Master Seed Script
-- =============================================
-- This script runs all seed scripts in the correct order
-- Execute this script to seed the entire database

PRINT 'Starting database seeding...';
PRINT '';

-- 1. Companies
PRINT '1. Seeding Companies...';
EXEC('$(SQLCMDPATH)\Database\Seed\01_Companies.sql');
PRINT '   ✓ Companies seeded';
PRINT '';

-- 2. Permissions
PRINT '2. Seeding Permissions...';
EXEC('$(SQLCMDPATH)\Database\Seed\02_Permissions.sql');
PRINT '   ✓ Permissions seeded';
PRINT '';

-- 3. Roles
PRINT '3. Seeding Roles...';
EXEC('$(SQLCMDPATH)\Database\Seed\03_Roles.sql');
PRINT '   ✓ Roles seeded';
PRINT '';

-- 4. Users
PRINT '4. Seeding Users...';
EXEC('$(SQLCMDPATH)\Database\Seed\04_Users.sql');
PRINT '   ✓ Users seeded';
PRINT '';

-- 5. UserRoles
PRINT '5. Seeding UserRoles...';
EXEC('$(SQLCMDPATH)\Database\Seed\05_UserRoles.sql');
PRINT '   ✓ UserRoles seeded';
PRINT '';

-- 6. RolePermissions
PRINT '6. Seeding RolePermissions...';
EXEC('$(SQLCMDPATH)\Database\Seed\06_RolePermissions.sql');
PRINT '   ✓ RolePermissions seeded';
PRINT '';

-- 7. UserCompanies
PRINT '7. Seeding UserCompanies...';
EXEC('$(SQLCMDPATH)\Database\Seed\07_UserCompanies.sql');
PRINT '   ✓ UserCompanies seeded';
PRINT '';

-- 8. Categories
PRINT '8. Seeding Categories...';
EXEC('$(SQLCMDPATH)\Database\Seed\08_Categories.sql');
PRINT '   ✓ Categories seeded';
PRINT '';

-- 9. Brands
PRINT '9. Seeding Brands...';
EXEC('$(SQLCMDPATH)\Database\Seed\09_Brands.sql');
PRINT '   ✓ Brands seeded';
PRINT '';

-- 10. Units of Measure
PRINT '10. Seeding Units of Measure...';
EXEC('$(SQLCMDPATH)\Database\Seed\10_UnitsOfMeasure.sql');
PRINT '   ✓ Units of Measure seeded';
PRINT '';

PRINT '========================================';
PRINT 'Database seeding completed successfully!';
PRINT '========================================';
PRINT '';
PRINT 'Default credentials:';
PRINT '  Admin: admin@khidmah.com / Admin@123';
PRINT '  Manager: manager@khidmah.com / Admin@123';
PRINT '  User: user@khidmah.com / Admin@123';
PRINT '';
PRINT 'NOTE: Please change the default passwords after first login!';

