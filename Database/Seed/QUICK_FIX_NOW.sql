-- =============================================
-- QUICK FIX: Update Admin Password Hash
-- =============================================
-- This is the SIMPLEST fix - just update the hash
-- =============================================

-- STEP 1: Get a fresh hash from your API
-- Run: cd Khidmah_Inventory.API && dotnet run
-- Copy the hash from console (starts with $2a$12$...)

-- STEP 2: Replace YOUR_HASH_HERE below with the hash you copied

-- STEP 3: Run this script

UPDATE Users
SET PasswordHash = 'YOUR_HASH_HERE',  -- ⚠️ REPLACE THIS!
    UpdatedAt = GETUTCDATE()
WHERE Email = 'admin@khidmah.com'
  AND IsDeleted = 0;

-- Also ensure UserCompany is set correctly
IF NOT EXISTS (
    SELECT 1 FROM UserCompanies 
    WHERE UserId = '20000000-0000-0000-0000-000000000001'
      AND CompanyId = '00000000-0000-0000-0000-000000000001'
)
BEGIN
    INSERT INTO UserCompanies (Id, CompanyId, UserId, IsDefault, IsActive, CreatedAt, IsDeleted)
    VALUES (
        NEWID(),
        '00000000-0000-0000-0000-000000000001',
        '20000000-0000-0000-0000-000000000001',
        1, 1, GETUTCDATE(), 0
    );
    PRINT 'UserCompany relationship created';
END
ELSE
BEGIN
    UPDATE UserCompanies
    SET IsDefault = 1,
        IsActive = 1,
        UpdatedAt = GETUTCDATE()
    WHERE UserId = '20000000-0000-0000-0000-000000000001'
      AND CompanyId = '00000000-0000-0000-0000-000000000001';
    PRINT 'UserCompany relationship updated';
END

-- Verify
SELECT 
    Email,
    CASE 
        WHEN PasswordHash LIKE '$2a$12$%' OR PasswordHash LIKE '$2a$11$%' THEN '✓ Hash looks valid'
        WHEN PasswordHash = 'YOUR_HASH_HERE' THEN '❌ NOT UPDATED - Replace YOUR_HASH_HERE'
        ELSE '⚠️  Check hash'
    END AS HashStatus,
    LEFT(PasswordHash, 40) + '...' AS HashPreview
FROM Users
WHERE Email = 'admin@khidmah.com';

PRINT '';
PRINT 'Done! Try logging in with: admin@khidmah.com / Admin@123';

