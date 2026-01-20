using BCrypt.Net;

namespace Khidmah_Inventory.API.Utilities;

/// <summary>
/// Temporary utility to generate password hashes for seed data
/// Add this to Program.cs temporarily, or run as a console app
/// </summary>
public class GeneratePasswordHashes
{
    public static void Run()
    {
        Console.WriteLine("=== BCrypt Password Hash Generator ===\n");
        
        // Default password from seed file
        string defaultPassword = "Admin@123";
        
        Console.WriteLine($"Generating hash for password: {defaultPassword}\n");
        
        // Generate hash with work factor 12 (same as IdentityService)
        var hash = BCrypt.Net.BCrypt.HashPassword(defaultPassword, BCrypt.Net.BCrypt.GenerateSalt(12));
        
        Console.WriteLine("Generated Hash:");
        Console.WriteLine(hash);
        Console.WriteLine("\nSQL Update Statement:");
        Console.WriteLine($"DECLARE @DefaultPasswordHash NVARCHAR(255) = '{hash}';");
        Console.WriteLine("\nVerification:");
        bool isValid = BCrypt.Net.BCrypt.Verify(defaultPassword, hash);
        Console.WriteLine($"Password verification: {(isValid ? "✓ SUCCESS" : "✗ FAILED")}");
        
        // Generate hashes for other common passwords if needed
        Console.WriteLine("\n\n=== Additional Hashes ===");
        var passwords = new[] { "Manager@123", "User@123", "Warehouse@123", "Sales@123" };
        foreach (var pwd in passwords)
        {
            var pwdHash = BCrypt.Net.BCrypt.HashPassword(pwd, BCrypt.Net.BCrypt.GenerateSalt(12));
            Console.WriteLine($"\nPassword: {pwd}");
            Console.WriteLine($"Hash: {pwdHash}");
        }
    }
}

