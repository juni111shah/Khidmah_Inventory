-- =============================================
-- FINAL FIX FOR LOGIN - COMPREHENSIVE
-- =============================================
-- This will fix ALL possible login issues
-- =============================================

DECLARE @AdminEmail NVARCHAR(255) = 'admin@khidmah.com';
DECLARE @AdminUserId UNIQUEIDENTIFIER = '20000000-0000-0000-0000-000000000001';
DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';

PRINT '========================================';
PRINT 'FINAL LOGIN FIX';
PRINT '========================================';
PRINT '';

-- =============================================
-- FIX 1: Ensure User is Active
-- =============================================
PRINT '1. Ensuring user is active...';
UPDATE Users
SET IsActive = 1,
    IsDeleted = 0,
    UpdatedAt = GETUTCDATE()
WHERE Email = @AdminEmail;
PRINT '   ✓ User status updated';

-- =============================================
-- FIX 2: Ensure Company is Active
-- =============================================
PRINT '';
PRINT '2. Ensuring company is active...';
UPDATE Companies
SET IsActive = 1,
    IsDeleted = 0,
    UpdatedAt = GETUTCDATE()
WHERE Id = @CompanyId;
PRINT '   ✓ Company status updated';

-- =============================================
-- FIX 3: Create/Fix UserCompany Relationship
-- =============================================
PRINT '';
PRINT '3. Fixing UserCompany relationship...';

-- Delete any existing UserCompany for this user/company
DELETE FROM UserCompanies
WHERE UserId = @AdminUserId
  AND CompanyId = @CompanyId;

-- Create new UserCompany with correct settings
INSERT INTO UserCompanies (Id, CompanyId, UserId, IsDefault, IsActive, CreatedAt, IsDeleted)
VALUES (
    NEWID(),
    @CompanyId,
    @AdminUserId,
    1,  -- IsDefault = TRUE
    1,  -- IsActive = TRUE
    GETUTCDATE(),
    0   -- IsDeleted = FALSE
);
PRINT '   ✓ UserCompany relationship created/fixed';

-- =============================================
-- FIX 4: Generate NEW Password Hash
-- =============================================
PRINT '';
PRINT '4. IMPORTANT: Password Hash Update';
PRINT '   ⚠️  You MUST get a FRESH hash from your API';
PRINT '   → Run: cd Khidmah_Inventory.API && dotnet run';
PRINT '   → Copy the hash from console output';
PRINT '   → The hash should be about 60 characters';
PRINT '   → It should start with $2a$12$...';
PRINT '';
PRINT '   Current hash in database:';
SELECT LEFT(PasswordHash, 60) + '...' AS CurrentHash
FROM Users
WHERE Email = @AdminEmail;
PRINT '';

-- =============================================
-- VERIFICATION
-- =============================================
PRINT '========================================';
PRINT 'VERIFICATION';
PRINT '========================================';

-- Check User
PRINT '';
PRINT 'User Status:';
SELECT 
    Email,
    IsActive,
    IsDeleted,
    CASE 
        WHEN IsActive = 1 AND IsDeleted = 0 THEN '✓ OK'
        ELSE '❌ FAIL'
    END AS Status
FROM Users
WHERE Email = @AdminEmail;

-- Check UserCompany
PRINT '';
PRINT 'UserCompany Status:';
SELECT 
    uc.UserId,
    uc.CompanyId,
    uc.IsDefault,
    uc.IsActive,
    CASE 
        WHEN uc.IsDefault = 1 AND uc.IsActive = 1 THEN '✓ OK'
        ELSE '❌ FAIL'
    END AS Status
FROM UserCompanies uc
WHERE uc.UserId = @AdminUserId
  AND uc.CompanyId = @CompanyId;

-- Check Company
PRINT '';
PRINT 'Company Status:';
SELECT 
    Id,
    Name,
    IsActive,
    IsDeleted,
    CASE 
        WHEN IsActive = 1 AND IsDeleted = 0 THEN '✓ OK'
        ELSE '❌ FAIL'
    END AS Status
FROM Companies
WHERE Id = @CompanyId;

PRINT '';
PRINT '========================================';
PRINT 'NEXT STEPS';
PRINT '========================================';
PRINT '';
PRINT '1. Get a FRESH password hash:';
PRINT '   → Run your API: dotnet run';
PRINT '   → Copy the hash from console';
PRINT '';
PRINT '2. Update the hash:';
PRINT '   → Run this SQL (replace YOUR_HASH with the hash you copied):';
PRINT '';
PRINT '   UPDATE Users';
PRINT '   SET PasswordHash = ''YOUR_HASH_HERE'',';
PRINT '       UpdatedAt = GETUTCDATE()';
PRINT '   WHERE Email = ''admin@khidmah.com'';';
PRINT '';
PRINT '3. Try logging in:';
PRINT '   → Email: admin@khidmah.com';
PRINT '   → Password: Admin@123';
PRINT '';
PRINT '========================================';












