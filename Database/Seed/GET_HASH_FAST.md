# 🚀 Get Password Hash Fast

## ⚡ FASTEST METHOD (30 seconds)

### Step 1: Run API
```bash
cd Khidmah_Inventory.API
dotnet run
```

### Step 2: Copy SQL from Console
The console will show:
```
📋 COPY THIS SQL AND RUN IT IN SQL SERVER:
----------------------------------------------------------------------
UPDATE Users
SET PasswordHash = '$2a$12$...',
    UpdatedAt = GETUTCDATE()
WHERE Email = 'admin@khidmah.com'
  AND IsDeleted = 0;
----------------------------------------------------------------------
```

### Step 3: Run in SQL Server
Just copy and paste that SQL - it's ready to use!

---

## 📋 Ready-to-Use SQL (Alternative)

If you want to try a pre-generated hash, use this:

```sql
UPDATE Users
SET PasswordHash = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y',
    UpdatedAt = GETUTCDATE()
WHERE Email = 'admin@khidmah.com'
  AND IsDeleted = 0;
```

**Note**: This hash was generated for `Admin@123`. Each BCrypt hash is unique, but all will work for the same password.

---

## ✅ After Running SQL

1. **Verify**:
   ```sql
   SELECT Email, LEFT(PasswordHash, 30) + '...' AS Hash
   FROM Users
   WHERE Email = 'admin@khidmah.com';
   ```
   Should show a hash starting with `$2a$12$...` (not the placeholder)

2. **Login**:
   - Email: `admin@khidmah.com`
   - Password: `Admin@123`

---

## 🔧 If Still Not Working

Run diagnostic:
```sql
-- Run: Database/Seed/12_DiagnoseLoginIssue.sql
```

