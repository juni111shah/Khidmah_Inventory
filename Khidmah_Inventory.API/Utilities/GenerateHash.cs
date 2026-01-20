using BCrypt.Net;

namespace Khidmah_Inventory.API.Utilities;

/// <summary>
/// Utility class to generate BCrypt password hash
/// This is not needed since Program.cs already generates the hash when you run the API
/// </summary>
public static class HashGenerator
{
    /// <summary>
    /// Generates and prints BCrypt hash for Admin@123
    /// </summary>
    public static void GenerateAndPrintHash(string password = "Admin@123")
    {
        // Generate hash with work factor 12 (same as IdentityService)
        string hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        
        Console.WriteLine("\n" + new string('=', 70));
        Console.WriteLine("BCRYPT PASSWORD HASH GENERATOR");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine($"Password: {password}");
        Console.WriteLine();
        Console.WriteLine("Generated Hash:");
        Console.WriteLine(hash);
        Console.WriteLine();
        Console.WriteLine("SQL UPDATE Statement:");
        Console.WriteLine(new string('-', 70));
        Console.WriteLine($"UPDATE Users");
        Console.WriteLine($"SET PasswordHash = '{hash}',");
        Console.WriteLine($"    UpdatedAt = GETUTCDATE()");
        Console.WriteLine($"WHERE Email = 'admin@khidmah.com'");
        Console.WriteLine($"  AND IsDeleted = 0;");
        Console.WriteLine(new string('-', 70));
        Console.WriteLine();
        
        // Verify
        bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
        Console.WriteLine($"Verification: {(isValid ? "✓ SUCCESS" : "✗ FAILED")}");
        Console.WriteLine(new string('=', 70));
        Console.WriteLine();
    }
}

