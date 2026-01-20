# How to Generate BCrypt Password Hashes

The placeholder hash `$2a$11$KIXLQZxJfNt5Y5Y5Y5Y5YOu5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y5Y` in the seed file is **NOT a valid hash** and will not work for authentication.

## Method 1: Using C# Code (Recommended)

### Option A: Add to Program.cs temporarily

Add this code to `Program.cs` before `app.Run()`:

```csharp
// TEMPORARY: Generate password hash - Remove after use
if (app.Environment.IsDevelopment())
{
    using (var scope = app.Services.CreateScope())
    {
        var identityService = scope.ServiceProvider.GetRequiredService<Khidmah_Inventory.Application.Common.Interfaces.IIdentityService>();
        var hash = await identityService.GeneratePasswordHashAsync("Admin@123");
        Console.WriteLine("\n=== PASSWORD HASH ===");
        Console.WriteLine($"Password: Admin@123");
        Console.WriteLine($"Hash: {hash}");
        Console.WriteLine("====================\n");
    }
}
```

Run the API and check the console output.

### Option B: Create a simple console app

Create a new console project or add this to a temporary file:

```csharp
using BCrypt.Net;

string password = "Admin@123";
string hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
Console.WriteLine($"Password: {password}");
Console.WriteLine($"Hash: {hash}");
```

## Method 2: Online BCrypt Generator

Use an online BCrypt generator:
- https://bcrypt-generator.com/
- https://www.bcrypt.fr/

**Settings:**
- Rounds: 12
- Password: `Admin@123`

## Method 3: Using PowerShell (if BCrypt module available)

```powershell
# Install BCrypt module if needed
# Install-Module BCrypt.Net-Next

$password = "Admin@123"
$hash = [BCrypt.Net.BCrypt]::HashPassword($password, [BCrypt.Net.BCrypt]::GenerateSalt(12))
Write-Host "Hash: $hash"
```

## Method 4: Using Node.js

```javascript
const bcrypt = require('bcrypt');
const password = 'Admin@123';
const hash = bcrypt.hashSync(password, 12);
console.log('Hash:', hash);
```

## Example Output

A valid BCrypt hash for "Admin@123" will look like:
```
$2a$12$LQv3c1yqBWVHxkd0LHAkCOYz6TtxMQJqhN8/LewY5GyY5Y5Y5Y5Y5Y
```

**Note:** Each time you generate a hash, it will be different (due to random salt), but all will verify correctly for the same password.

## Updating the Seed File

Once you have the hash, update `04_Users.sql`:

```sql
DECLARE @DefaultPasswordHash NVARCHAR(255) = '$2a$12$YOUR_GENERATED_HASH_HERE';
```

## Verification

After updating the seed file, you can verify the hash works by:

1. Running the seed script
2. Attempting to login with:
   - Email: `admin@khidmah.com`
   - Password: `Admin@123`

## Security Note

⚠️ **IMPORTANT**: Change all default passwords immediately after first login!

