using BCrypt.Net;

namespace Khidmah_Inventory.API.Utilities;

/// <summary>
/// Standalone utility to generate BCrypt password hash
/// You can call this from Program.cs temporarily or create a console app
/// </summary>
public static class GenerateHashConsole
{
    /// <summary>
    /// Generates and prints BCrypt hash for a password
    /// </summary>
    public static void GenerateAndPrintHash(string password = "Admin@123")
    {
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("BCRYPT PASSWORD HASH GENERATOR");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"Password: {password}");
        Console.WriteLine();
        
        // Generate hash with work factor 12 (same as IdentityService)
        var hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        
        Console.WriteLine("Generated Hash:");
        Console.WriteLine(hash);
        Console.WriteLine();
        Console.WriteLine("SQL Update Statement:");
        Console.WriteLine($"UPDATE Users SET PasswordHash = '{hash}' WHERE Email = 'admin@khidmah.com' AND IsDeleted = 0;");
        Console.WriteLine();
        Console.WriteLine("Or use in 11_UpdatePasswordHash.sql:");
        Console.WriteLine($"DECLARE @PasswordHash NVARCHAR(255) = '{hash}';");
        Console.WriteLine();
        
        // Verify the hash
        bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
        Console.WriteLine($"Verification: {(isValid ? "✓ SUCCESS - Hash is valid" : "✗ FAILED - Hash is invalid")}");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine();
    }
}

