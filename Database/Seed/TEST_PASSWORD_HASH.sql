-- =============================================
-- TEST: Check if Password Hash is Valid
-- =============================================
-- This helps verify if the hash format is correct
-- =============================================

DECLARE @AdminEmail NVARCHAR(255) = 'admin@khidmah.com';
DECLARE @TestHash NVARCHAR(255) = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y';

PRINT '========================================';
PRINT 'PASSWORD HASH ANALYSIS';
PRINT '========================================';
PRINT '';

-- Check current hash in database
SELECT 
    Email,
    PasswordHash,
    LEN(PasswordHash) AS HashLength,
    CASE 
        WHEN PasswordHash LIKE '$2a$11$%' THEN 'BCrypt $2a$11$ (work factor 11)'
        WHEN PasswordHash LIKE '$2a$12$%' THEN 'BCrypt $2a$12$ (work factor 12)'
        WHEN PasswordHash LIKE '$2a$13$%' THEN 'BCrypt $2a$13$ (work factor 13)'
        WHEN PasswordHash = '$2a$11$KIXLQZxJfNt5Y5Y5Y5Y5YOu5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y' THEN '❌ PLACEHOLDER'
        WHEN PasswordHash = 'REPLACE_WITH_HASH_FROM_API_CONSOLE' THEN '❌ NOT SET'
        ELSE '⚠️  UNKNOWN FORMAT'
    END AS HashType,
    CASE 
        WHEN LEN(PasswordHash) BETWEEN 55 AND 65 THEN '✓ Length OK'
        ELSE '❌ Length suspicious'
    END AS LengthCheck,
    CASE 
        WHEN PasswordHash LIKE '%Y5Y5Y5Y5Y5Y%' THEN '⚠️  Has repeated pattern (suspicious)'
        ELSE '✓ No obvious pattern'
    END AS PatternCheck
FROM Users
WHERE Email = @AdminEmail;

PRINT '';
PRINT '========================================';
PRINT 'ISSUES TO CHECK:';
PRINT '========================================';
PRINT '';
PRINT '1. Hash Length: Should be 60 characters';
PRINT '2. Hash Format: Should start with $2a$12$ or $2a$11$';
PRINT '3. Hash Pattern: Should NOT have obvious repeating patterns';
PRINT '4. Hash Source: Must be generated for password "Admin@123"';
PRINT '';
PRINT 'The hash you showed has repeated "Y5Y5Y5Y5Y5Y" which is suspicious.';
PRINT 'This suggests it might not be a valid BCrypt hash.';
PRINT '';
PRINT 'SOLUTION: Generate a FRESH hash from your API.';
PRINT 'The API uses BCrypt.Net which will generate a proper hash.';
PRINT '';












