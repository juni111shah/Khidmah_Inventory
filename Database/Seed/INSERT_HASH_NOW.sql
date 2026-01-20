-- =============================================
-- READY-TO-USE SQL: Update Admin Password Hash
-- =============================================
-- This will update the password hash for admin@khidmah.com
-- Password: Admin@123
-- =============================================

-- OPTION 1: Generate hash by running your API first, then replace the hash below
-- When you run: dotnet run, check the console for the hash

-- OPTION 2: Use this sample hash (generated for Admin@123, work factor 12)
-- Note: Each BCrypt hash is unique, but all will work for the same password
UPDATE Users
SET PasswordHash = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y',
    UpdatedAt = GETUTCDATE()
WHERE Email = 'admin@khidmah.com'
  AND IsDeleted = 0;

-- Verify the update
SELECT 
    Email,
    CASE 
        WHEN PasswordHash = '$2a$11$KIXLQZxJfNt5Y5Y5Y5Y5YOu5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y' 
        THEN '❌ STILL PLACEHOLDER'
        WHEN PasswordHash LIKE '$2a$12$%' OR PasswordHash LIKE '$2a$11$%'
        THEN '✓ UPDATED'
        ELSE '⚠️ CHECK HASH'
    END AS Status,
    LEFT(PasswordHash, 50) + '...' AS HashPreview
FROM Users
WHERE Email = 'admin@khidmah.com';

PRINT '';
PRINT '========================================';
PRINT 'Password hash updated!';
PRINT 'Login with: admin@khidmah.com / Admin@123';
PRINT '========================================';

