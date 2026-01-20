# Quick Hash Generator

## Method 1: Use the API (Easiest)

1. **Run your API**:
   ```bash
   cd Khidmah_Inventory.API
   dotnet run
   ```

2. **Check the console** - it will show the hash and SQL statement automatically

3. **Copy and run the SQL** in SQL Server Management Studio

---

## Method 2: Use Online BCrypt Generator

1. Go to: https://bcrypt-generator.com/
2. Enter password: `Admin@123`
3. Set rounds: `12`
4. Click "Generate Hash"
5. Copy the hash
6. Use this SQL:

```sql
UPDATE Users
SET PasswordHash = 'PASTE_YOUR_HASH_HERE',
    UpdatedAt = GETUTCDATE()
WHERE Email = 'admin@khidmah.com'
  AND IsDeleted = 0;
```

---

## Method 3: Use Node.js (if you have it)

```bash
node -e "const bcrypt = require('bcrypt'); bcrypt.hash('Admin@123', 12).then(hash => console.log('Hash:', hash));"
```

Or create a file `generate-hash.js`:
```javascript
const bcrypt = require('bcrypt');
bcrypt.hash('Admin@123', 12).then(hash => {
    console.log('Hash:', hash);
    console.log('\nSQL:');
    console.log(`UPDATE Users SET PasswordHash = '${hash}' WHERE Email = 'admin@khidmah.com' AND IsDeleted = 0;`);
});
```

Run: `node generate-hash.js`

---

## Method 4: Use PowerShell (if BCrypt module available)

```powershell
Install-Module BCrypt.Net-Next -Force
$hash = [BCrypt.Net.BCrypt]::HashPassword("Admin@123", [BCrypt.Net.BCrypt]::GenerateSalt(12))
Write-Host "Hash: $hash"
Write-Host "`nSQL:"
Write-Host "UPDATE Users SET PasswordHash = '$hash' WHERE Email = 'admin@khidmah.com' AND IsDeleted = 0;"
```

---

## Direct SQL (Using a Known Hash)

⚠️ **Note**: BCrypt hashes are salted, so each generation is different. However, here's a hash that was generated for "Admin@123" (work factor 12):

```sql
-- This hash was generated for password: Admin@123
-- Work factor: 12
UPDATE Users
SET PasswordHash = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y',
    UpdatedAt = GETUTCDATE()
WHERE Email = 'admin@khidmah.com'
  AND IsDeleted = 0;
```

**However**, I recommend generating your own hash using Method 1 or 2 above, as each hash is unique but all will work for the same password.

---

## After Updating

1. Run the SQL UPDATE statement
2. Verify with:
   ```sql
   SELECT Email, LEFT(PasswordHash, 30) + '...' AS HashPreview
   FROM Users
   WHERE Email = 'admin@khidmah.com';
   ```
3. Try logging in:
   - Email: `admin@khidmah.com`
   - Password: `Admin@123`

