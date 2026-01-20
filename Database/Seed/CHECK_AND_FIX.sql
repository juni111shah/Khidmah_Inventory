-- =============================================
-- SIMPLE CHECK AND FIX FOR LOGIN
-- =============================================

DECLARE @AdminEmail NVARCHAR(255) = 'admin@khidmah.com';
DECLARE @AdminUserId UNIQUEIDENTIFIER = '20000000-0000-0000-0000-000000000001';
DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';

PRINT '========================================';
PRINT 'CHECKING LOGIN ISSUES...';
PRINT '========================================';
PRINT '';

-- Check 1: User exists and is active
PRINT '1. User Status:';
SELECT 
    Email,
    IsActive,
    IsDeleted,
    CASE 
        WHEN IsActive = 1 AND IsDeleted = 0 THEN '✓ OK'
        ELSE '❌ PROBLEM'
    END AS Status
FROM Users
WHERE Email = @AdminEmail;

-- Check 2: Password Hash
PRINT '';
PRINT '2. Password Hash:';
SELECT 
    Email,
    LEFT(PasswordHash, 50) + '...' AS HashPreview,
    CASE 
        WHEN PasswordHash = '$2a$11$KIXLQZxJfNt5Y5Y5Y5Y5YOu5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y' THEN '❌ PLACEHOLDER'
        WHEN PasswordHash = 'REPLACE_WITH_HASH_FROM_API_CONSOLE' THEN '❌ NOT SET'
        WHEN PasswordHash LIKE '$2a$12$%' OR PasswordHash LIKE '$2a$11$%' THEN '✓ Looks valid (but may not match password)'
        ELSE '⚠️  UNKNOWN FORMAT'
    END AS HashStatus
FROM Users
WHERE Email = @AdminEmail;

-- Check 3: UserCompany
PRINT '';
PRINT '3. UserCompany Relationship:';
SELECT 
    uc.UserId,
    uc.CompanyId,
    uc.IsDefault,
    uc.IsActive,
    CASE 
        WHEN uc.IsDefault = 1 AND uc.IsActive = 1 THEN '✓ OK'
        ELSE '❌ PROBLEM - Will cause login to fail!'
    END AS Status
FROM UserCompanies uc
WHERE uc.UserId = @AdminUserId
  AND uc.CompanyId = @CompanyId;

-- Check 4: Company
PRINT '';
PRINT '4. Company Status:';
SELECT 
    Id,
    Name,
    IsActive,
    IsDeleted,
    CASE 
        WHEN IsActive = 1 AND IsDeleted = 0 THEN '✓ OK'
        ELSE '❌ PROBLEM'
    END AS Status
FROM Companies
WHERE Id = @CompanyId;

PRINT '';
PRINT '========================================';
PRINT 'MOST LIKELY ISSUES:';
PRINT '========================================';
PRINT '';
PRINT '1. Password hash doesn''t match "Admin@123"';
PRINT '   → Solution: Get a FRESH hash from API and update';
PRINT '   → Run: cd Khidmah_Inventory.API && dotnet run';
PRINT '   → Copy the hash from console output';
PRINT '';
PRINT '2. UserCompany missing or IsDefault = 0 or IsActive = 0';
PRINT '   → Solution: Run the fix below';
PRINT '';
PRINT '========================================';
PRINT 'AUTO-FIX: UserCompany';
PRINT '========================================';

-- Auto-fix UserCompany
IF NOT EXISTS (
    SELECT 1 FROM UserCompanies 
    WHERE UserId = @AdminUserId AND CompanyId = @CompanyId
)
BEGIN
    INSERT INTO UserCompanies (Id, CompanyId, UserId, IsDefault, IsActive, CreatedAt, IsDeleted)
    VALUES (NEWID(), @CompanyId, @AdminUserId, 1, 1, GETUTCDATE(), 0);
    PRINT '✓ UserCompany created';
END
ELSE
BEGIN
    UPDATE UserCompanies
    SET IsDefault = 1,
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE UserId = @AdminUserId
      AND CompanyId = @CompanyId;
    PRINT '✓ UserCompany fixed';
END

PRINT '';
PRINT '========================================';
PRINT 'NEXT STEP: Update Password Hash';
PRINT '========================================';
PRINT '';
PRINT 'Run your API and get a fresh hash, then run:';
PRINT '';
PRINT 'UPDATE Users';
PRINT 'SET PasswordHash = ''PASTE_HASH_HERE'',';
PRINT '    UpdatedAt = GETUTCDATE()';
PRINT 'WHERE Email = ''admin@khidmah.com'';';
PRINT '';

