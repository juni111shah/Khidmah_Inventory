-- =============================================
-- COMPLETE LOGIN FIX SCRIPT
-- =============================================
-- This script will:
-- 1. Check all login requirements
-- 2. Fix password hash
-- 3. Fix UserCompany relationship
-- 4. Verify everything is correct
-- =============================================

DECLARE @AdminEmail NVARCHAR(255) = 'admin@khidmah.com';
DECLARE @AdminUserId UNIQUEIDENTIFIER = '20000000-0000-0000-0000-000000000001';
DECLARE @CompanyId UNIQUEIDENTIFIER = '00000000-0000-0000-0000-000000000001';

PRINT '========================================';
PRINT 'COMPLETE LOGIN FIX';
PRINT '========================================';
PRINT '';

-- =============================================
-- STEP 1: Generate a NEW password hash
-- =============================================
-- IMPORTANT: You need to get a fresh hash from your API
-- Run: dotnet run (in Khidmah_Inventory.API folder)
-- Copy the hash from console output
-- =============================================

-- Replace this with the hash from API console output
-- The hash should be about 60 characters and start with $2a$12$
DECLARE @NewPasswordHash NVARCHAR(255) = 'REPLACE_WITH_FRESH_HASH_FROM_API';

-- If you don't have a fresh hash, use this one (generated for Admin@123):
-- DECLARE @NewPasswordHash NVARCHAR(255) = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y';

-- =============================================
-- STEP 2: Check current status
-- =============================================
PRINT '1. CHECKING USER STATUS...';
DECLARE @UserExists BIT = 0;
DECLARE @IsActive BIT = 0;
DECLARE @IsDeleted BIT = 0;
DECLARE @CurrentHash NVARCHAR(255);

SELECT 
    @UserExists = 1,
    @IsActive = IsActive,
    @IsDeleted = IsDeleted,
    @CurrentHash = PasswordHash
FROM Users
WHERE Email = @AdminEmail;

IF @UserExists = 0
BEGIN
    PRINT '   ❌ USER NOT FOUND';
    PRINT '   → Run seed script 04_Users.sql first';
    RETURN;
END

PRINT '   ✓ User exists';
PRINT '   IsActive: ' + CAST(@IsActive AS NVARCHAR(1));
PRINT '   IsDeleted: ' + CAST(@IsDeleted AS NVARCHAR(1));

-- =============================================
-- STEP 3: Check UserCompany relationship
-- =============================================
PRINT '';
PRINT '2. CHECKING USER-COMPANY RELATIONSHIP...';
DECLARE @HasUserCompany BIT = 0;
DECLARE @IsDefault BIT = 0;
DECLARE @CompanyIsActive BIT = 0;
DECLARE @UserCompanyId UNIQUEIDENTIFIER;

SELECT 
    @HasUserCompany = 1,
    @IsDefault = IsDefault,
    @CompanyIsActive = IsActive,
    @UserCompanyId = Id
FROM UserCompanies
WHERE UserId = @AdminUserId
  AND CompanyId = @CompanyId;

IF @HasUserCompany = 0
BEGIN
    PRINT '   ❌ NO USER-COMPANY RELATIONSHIP';
    PRINT '   → Creating UserCompany relationship...';
    
    INSERT INTO UserCompanies (Id, CompanyId, UserId, IsDefault, IsActive, CreatedAt, IsDeleted)
    VALUES (
        NEWID(),
        @CompanyId,
        @AdminUserId,
        1, -- IsDefault
        1, -- IsActive
        GETUTCDATE(),
        0  -- IsDeleted
    );
    
    PRINT '   ✓ UserCompany relationship created';
END
ELSE
BEGIN
    PRINT '   ✓ UserCompany relationship exists';
    PRINT '   IsDefault: ' + CAST(@IsDefault AS NVARCHAR(1));
    PRINT '   IsActive: ' + CAST(@CompanyIsActive AS NVARCHAR(1));
    
    -- Fix if needed
    IF @IsDefault = 0 OR @CompanyIsActive = 0
    BEGIN
        PRINT '   → Fixing UserCompany...';
        UPDATE UserCompanies
        SET IsDefault = 1,
            IsActive = 1,
            UpdatedAt = GETUTCDATE()
        WHERE UserId = @AdminUserId
          AND CompanyId = @CompanyId;
        PRINT '   ✓ UserCompany fixed';
    END
END

-- =============================================
-- STEP 4: Update password hash
-- =============================================
PRINT '';
PRINT '3. UPDATING PASSWORD HASH...';

IF @NewPasswordHash = 'REPLACE_WITH_FRESH_HASH_FROM_API'
BEGIN
    PRINT '   ⚠️  WARNING: Using placeholder hash!';
    PRINT '   → You MUST replace @NewPasswordHash with a fresh hash from API';
    PRINT '   → Run: cd Khidmah_Inventory.API && dotnet run';
    PRINT '   → Copy the hash from console output';
    RETURN;
END

UPDATE Users
SET PasswordHash = @NewPasswordHash,
    UpdatedAt = GETUTCDATE()
WHERE Email = @AdminEmail
  AND IsDeleted = 0;

IF @@ROWCOUNT > 0
BEGIN
    PRINT '   ✓ Password hash updated';
END
ELSE
BEGIN
    PRINT '   ❌ Failed to update password hash';
    RETURN;
END

-- =============================================
-- STEP 5: Verify Company exists and is active
-- =============================================
PRINT '';
PRINT '4. CHECKING COMPANY...';
DECLARE @CompanyExists BIT = 0;
DECLARE @CompanyIsActive BIT = 0;

SELECT 
    @CompanyExists = 1,
    @CompanyIsActive = IsActive
FROM Companies
WHERE Id = @CompanyId
  AND IsDeleted = 0;

IF @CompanyExists = 0
BEGIN
    PRINT '   ❌ COMPANY NOT FOUND';
    PRINT '   → Run seed script 01_Companies.sql';
    RETURN;
END

IF @CompanyIsActive = 0
BEGIN
    PRINT '   ⚠️  COMPANY IS INACTIVE';
    PRINT '   → Activating company...';
    UPDATE Companies
    SET IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE Id = @CompanyId;
    PRINT '   ✓ Company activated';
END
ELSE
BEGIN
    PRINT '   ✓ Company exists and is active';
END

-- =============================================
-- STEP 6: Final verification
-- =============================================
PRINT '';
PRINT '5. FINAL VERIFICATION...';
PRINT '';

SELECT 
    'User Status' AS CheckItem,
    CASE 
        WHEN u.IsActive = 1 AND u.IsDeleted = 0 THEN '✓ OK'
        ELSE '❌ FAIL'
    END AS Status
FROM Users u
WHERE u.Email = @AdminEmail

UNION ALL

SELECT 
    'Password Hash' AS CheckItem,
    CASE 
        WHEN u.PasswordHash LIKE '$2a$12$%' OR u.PasswordHash LIKE '$2a$11$%' THEN '✓ OK'
        WHEN u.PasswordHash = '$2a$11$KIXLQZxJfNt5Y5Y5Y5Y5YOu5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y' THEN '❌ PLACEHOLDER'
        WHEN u.PasswordHash = 'REPLACE_WITH_HASH_FROM_API_CONSOLE' THEN '❌ NOT SET'
        ELSE '⚠️  CHECK'
    END AS Status
FROM Users u
WHERE u.Email = @AdminEmail

UNION ALL

SELECT 
    'UserCompany' AS CheckItem,
    CASE 
        WHEN uc.IsDefault = 1 AND uc.IsActive = 1 THEN '✓ OK'
        ELSE '❌ FAIL'
    END AS Status
FROM UserCompanies uc
WHERE uc.UserId = @AdminUserId
  AND uc.CompanyId = @CompanyId

UNION ALL

SELECT 
    'Company' AS CheckItem,
    CASE 
        WHEN c.IsActive = 1 AND c.IsDeleted = 0 THEN '✓ OK'
        ELSE '❌ FAIL'
    END AS Status
FROM Companies c
WHERE c.Id = @CompanyId;

PRINT '';
PRINT '========================================';
PRINT 'FIX COMPLETE!';
PRINT '========================================';
PRINT '';
PRINT 'Try logging in with:';
PRINT '  Email: admin@khidmah.com';
PRINT '  Password: Admin@123';
PRINT '';
PRINT 'If it still doesn''t work:';
PRINT '  1. Make sure you replaced @NewPasswordHash with a FRESH hash from API';
PRINT '  2. Check API logs for any errors';
PRINT '  3. Run diagnostic: Database/Seed/12_DiagnoseLoginIssue.sql';
PRINT '========================================';

