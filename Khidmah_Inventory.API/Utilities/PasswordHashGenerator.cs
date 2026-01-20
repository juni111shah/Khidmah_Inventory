using BCrypt.Net;

namespace Khidmah_Inventory.API.Utilities;

/// <summary>
/// Utility class to generate BCrypt password hashes for seed data
/// Run this in a console or use it in Program.cs temporarily to generate hashes
/// </summary>
public static class PasswordHashGenerator
{
    /// <summary>
    /// Generates a BCrypt hash for the given password
    /// </summary>
    /// <param name="password">The plain text password</param>
    /// <param name="workFactor">BCrypt work factor (default: 12)</param>
    /// <returns>The BCrypt hash string</returns>
    public static string GenerateHash(string password, int workFactor = 12)
    {
        var salt = BCrypt.Net.BCrypt.GenerateSalt(workFactor);
        return BCrypt.Net.BCrypt.HashPassword(password, salt);
    }

    /// <summary>
    /// Verifies a password against a hash
    /// </summary>
    /// <param name="password">The plain text password</param>
    /// <param name="hash">The BCrypt hash</param>
    /// <returns>True if password matches</returns>
    public static bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}

