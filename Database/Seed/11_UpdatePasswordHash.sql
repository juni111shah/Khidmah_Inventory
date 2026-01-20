-- =============================================
-- Update Password Hash for Admin User
-- =============================================
-- This script updates the password hash for admin@khidmah.com
-- 
-- INSTRUCTIONS:
-- 1. Run your API (dotnet run) and check the console output
-- 2. Copy the hash that starts with $2a$12$...
-- 3. Replace YOUR_HASH_HERE below with that hash
-- 4. Run this script
-- =============================================

-- STEP 1: Get hash from API console output when you run: dotnet run
-- The hash will look like: $2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y
-- Copy the ENTIRE hash and paste it below (including the $2a$12$ part)

DECLARE @PasswordHash NVARCHAR(255) = 'YOUR_HASH_HERE'; -- ⚠️ REPLACE THIS with hash from API console

-- STEP 2: Update the password hash
UPDATE Users
SET PasswordHash = @PasswordHash,
    UpdatedAt = GETUTCDATE()
WHERE Email = 'admin@khidmah.com'
  AND IsDeleted = 0;

-- STEP 3: Verify the update
SELECT 
    Email,
    UserName,
    CASE 
        WHEN PasswordHash = '$2a$11$KIXLQZxJfNt5Y5Y5Y5Y5YOu5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y' 
        THEN '❌ STILL PLACEHOLDER - NOT UPDATED'
        WHEN PasswordHash = 'YOUR_HASH_HERE'
        THEN '❌ NOT UPDATED - Replace YOUR_HASH_HERE with actual hash'
        ELSE '✓ UPDATED'
    END AS Status,
    LEFT(PasswordHash, 40) + '...' AS HashPreview
FROM Users
WHERE Email = 'admin@khidmah.com';

PRINT '';
PRINT '========================================';
PRINT 'Password hash updated!';
PRINT 'Try logging in with:';
PRINT '  Email: admin@khidmah.com';
PRINT '  Password: Admin@123';
PRINT '========================================';

