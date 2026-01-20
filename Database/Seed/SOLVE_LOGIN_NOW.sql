-- =============================================
-- SOLVE LOGIN ISSUE - SIMPLE & DIRECT
-- =============================================
-- This hash has a suspicious pattern and likely doesn't match "Admin@123"
-- =============================================

DECLARE @AdminEmail NVARCHAR(255) = 'admin@khidmah.com';
DECLARE @AdminUserId UNIQUEIDENTIFIER = '20000000-0000-0000-0000-000000000001';
DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';

PRINT '========================================';
PRINT 'SOLVING LOGIN ISSUE';
PRINT '========================================';
PRINT '';

-- =============================================
-- FIX 1: UserCompany (Most Common Issue)
-- =============================================
PRINT '1. Fixing UserCompany relationship...';

-- Remove any existing
DELETE FROM UserCompanies
WHERE UserId = @AdminUserId AND CompanyId = @CompanyId;

-- Create correct one
INSERT INTO UserCompanies (Id, CompanyId, UserId, IsDefault, IsActive, CreatedAt, IsDeleted)
VALUES (NEWID(), @CompanyId, @AdminUserId, 1, 1, GETUTCDATE(), 0);

PRINT '   ✓ UserCompany fixed (IsDefault=1, IsActive=1)';

-- =============================================
-- FIX 2: User Status
-- =============================================
PRINT '';
PRINT '2. Ensuring user is active...';
UPDATE Users SET IsActive = 1, IsDeleted = 0 WHERE Email = @AdminEmail;
PRINT '   ✓ User is active';

-- =============================================
-- FIX 3: Company Status
-- =============================================
PRINT '';
PRINT '3. Ensuring company is active...';
UPDATE Companies SET IsActive = 1, IsDeleted = 0 WHERE Id = @CompanyId;
PRINT '   ✓ Company is active';

-- =============================================
-- FIX 4: Password Hash (YOU MUST DO THIS)
-- =============================================
PRINT '';
PRINT '========================================';
PRINT 'CRITICAL: PASSWORD HASH';
PRINT '========================================';
PRINT '';
PRINT 'The current hash has a suspicious pattern and likely doesn''t work.';
PRINT '';
PRINT 'YOU MUST:';
PRINT '  1. Run your API: cd Khidmah_Inventory.API && dotnet run';
PRINT '  2. Find the hash in console output (starts with $2a$12$...)';
PRINT '  3. Copy the ENTIRE hash (should be 60 characters, no repeating patterns)';
PRINT '  4. Run this SQL (replace YOUR_HASH_HERE):';
PRINT '';
PRINT '  UPDATE Users';
PRINT '  SET PasswordHash = ''YOUR_HASH_HERE'',';
PRINT '      UpdatedAt = GETUTCDATE()';
PRINT '  WHERE Email = ''admin@khidmah.com'';';
PRINT '';
PRINT 'The hash from API will look like:';
PRINT '  $2a$12$abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUV';
PRINT '  (60 characters, no obvious patterns)';
PRINT '';
PRINT 'NOT like your current hash which has: Y5Y5Y5Y5Y5Y (repeating pattern)';
PRINT '';

-- =============================================
-- VERIFICATION
-- =============================================
PRINT '========================================';
PRINT 'CURRENT STATUS';
PRINT '========================================';

SELECT 
    'User' AS Item,
    CASE WHEN IsActive = 1 AND IsDeleted = 0 THEN '✓ OK' ELSE '❌ FIX' END AS Status
FROM Users WHERE Email = @AdminEmail

UNION ALL

SELECT 
    'UserCompany' AS Item,
    CASE WHEN IsDefault = 1 AND IsActive = 1 THEN '✓ OK' ELSE '❌ FIX' END AS Status
FROM UserCompanies WHERE UserId = @AdminUserId AND CompanyId = @CompanyId

UNION ALL

SELECT 
    'Company' AS Item,
    CASE WHEN IsActive = 1 AND IsDeleted = 0 THEN '✓ OK' ELSE '❌ FIX' END AS Status
FROM Companies WHERE Id = @CompanyId

UNION ALL

SELECT 
    'Password Hash' AS Item,
    CASE 
        WHEN PasswordHash LIKE '%Y5Y5Y5Y5Y5Y%' THEN '❌ SUSPICIOUS - Needs replacement'
        WHEN PasswordHash LIKE '$2a$12$%' AND LEN(PasswordHash) = 60 THEN '✓ Format OK'
        ELSE '⚠️  Check hash'
    END AS Status
FROM Users WHERE Email = @AdminEmail;

PRINT '';
PRINT '========================================';
PRINT 'NEXT STEP';
PRINT '========================================';
PRINT 'Get a fresh hash from API and update it!';
PRINT '========================================';












