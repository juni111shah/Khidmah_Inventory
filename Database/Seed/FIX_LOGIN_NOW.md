# 🔧 Quick Fix: Password Hash Not Matching

## The Problem
The password hash in the database is a placeholder: `$2a$11$KIXLQZxJfNt5Y5Y5Y5Y5YOu5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y`

This placeholder **does NOT match** the password `Admin@123`.

## ✅ Solution (3 Simple Steps)

### Step 1: Run Your API
```bash
cd Khidmah_Inventory.API
dotnet run
```

### Step 2: Copy the Hash from Console
When the API starts, you'll see output like this in the console:

```
======================================================================
🔐 PASSWORD HASH FOR ADMIN USER
======================================================================
Password: Admin@123
Hash: $2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y

📋 COPY THIS SQL AND RUN IT IN SQL SERVER:
----------------------------------------------------------------------
UPDATE Users
SET PasswordHash = '$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y',
    UpdatedAt = GETUTCDATE()
WHERE Email = 'admin@khidmah.com'
  AND IsDeleted = 0;
----------------------------------------------------------------------
```

**Copy the entire SQL UPDATE statement** (the part between the dashes).

### Step 3: Run the SQL in SQL Server
1. Open SQL Server Management Studio
2. Connect to your database
3. Paste and run the SQL UPDATE statement you copied
4. You should see: `(1 row affected)`

### Step 4: Test Login
Try logging in again:
- **Email**: `admin@khidmah.com`
- **Password**: `Admin@123`

It should work now! ✅

---

## Alternative: Manual Method

If you prefer to do it manually:

1. **Get the hash** from API console (the long string starting with `$2a$12$...`)

2. **Open** `Database/Seed/11_UpdatePasswordHash.sql`

3. **Replace** `YOUR_HASH_HERE` with the hash you copied

4. **Run** the script in SQL Server

---

## Still Not Working?

Run the diagnostic script to check for other issues:
```sql
-- Run: Database/Seed/12_DiagnoseLoginIssue.sql
```

This will check:
- ✅ User exists
- ✅ User is active
- ✅ Password hash is valid
- ✅ User has a default company
- ✅ Company is active

---

## Notes

- Each time you generate a hash, it will be **different** (due to random salt)
- But **all hashes** for the same password will work correctly
- The hash in the database must match the password you're trying to login with

