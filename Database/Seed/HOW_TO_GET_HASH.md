# How to Get a Valid Password Hash

## The Problem

Your current hash `$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y` **does NOT match** the password `Admin@123`.

The hash has a suspicious repeating pattern (`Y5Y5Y5Y5Y5Y`) which means it's invalid or corrupted.

## ✅ Solution: Get a Fresh Hash from Your API

### Step 1: Run Your API

Open a terminal/command prompt and run:

```bash
cd Khidmah_Inventory.API
dotnet run
```

### Step 2: Find the Hash in Console

When the API starts, you'll see output like this:

```
======================================================================
🔐 PASSWORD HASH FOR ADMIN USER
======================================================================
Password: Admin@123
Hash: $2a$12$abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUV

📋 COPY THIS SQL AND RUN IT IN SQL SERVER:
----------------------------------------------------------------------
UPDATE Users
SET PasswordHash = '$2a$12$abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUV',
    UpdatedAt = GETUTCDATE()
WHERE Email = 'admin@khidmah.com'
  AND IsDeleted = 0;
----------------------------------------------------------------------
```

### Step 3: Copy the Hash

**Option A: Copy the entire SQL statement** (easiest)
- Copy everything between the dashes (`------`)
- Paste and run it directly in SQL Server Management Studio

**Option B: Copy just the hash**
- Copy the hash value (the long string starting with `$2a$12$...`)
- Use it in `Database/Seed/UPDATE_HASH_SIMPLE.sql`

### Step 4: Update the Database

**If you copied the SQL statement:**
- Paste it in SQL Server Management Studio
- Run it (F5 or Execute)
- Done! ✅

**If you copied just the hash:**
1. Open `Database/Seed/UPDATE_HASH_SIMPLE.sql`
2. Replace `YOUR_HASH_HERE` with the hash you copied
3. Run the script

### Step 5: Test Login

Try logging in:
- **Email**: `admin@khidmah.com`
- **Password**: `Admin@123`

It should work now! ✅

---

## 🔍 What a Valid Hash Looks Like

A valid BCrypt hash for "Admin@123" should:
- ✅ Start with `$2a$12$` (or `$2a$11$`)
- ✅ Be exactly **60 characters** long
- ✅ Look **random** (no obvious patterns)
- ✅ Have **NO repeating sequences** like `Y5Y5Y5Y5Y5Y`

**Example of a valid hash:**
```
$2a$12$abcdefghijklmnopqrstuvwxyz1234567890ABCDEFGHIJKLMNOPQRSTUV
```

**Your current hash (INVALID):**
```
$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y
                                                      ^^^^^^^^^^^
                                                      Repeating pattern!
```

---

## 🚨 Still Not Working?

If login still fails after updating:

1. **Double-check the hash** - Make sure you copied the ENTIRE hash (all 60 characters)
2. **Check for spaces** - Make sure there are no extra spaces before/after the hash
3. **Verify in database**:
   ```sql
   SELECT Email, PasswordHash, LEN(PasswordHash) AS Length
   FROM Users
   WHERE Email = 'admin@khidmah.com';
   ```
   Should show: Length = 60
4. **Check UserCompany** - Make sure it exists with IsDefault=1 and IsActive=1:
   ```sql
   SELECT * FROM UserCompanies
   WHERE UserId = '20000000-0000-0000-0000-000000000001';
   ```

---

## 📝 Summary

**The issue**: Your hash doesn't match "Admin@123"

**The solution**: Get a fresh hash from your API (it generates it correctly)

**The steps**: Run API → Copy hash → Update database → Login

That's it! 🎉












