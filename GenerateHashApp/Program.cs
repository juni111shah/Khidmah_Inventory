using BCrypt.Net;

Console.WriteLine("============================================");
Console.WriteLine("PASSWORD HASH GENERATOR");
Console.WriteLine("============================================");
Console.WriteLine();

string password = "Admin@123";
string hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));

Console.WriteLine($"Password: {password}");
Console.WriteLine();
Console.WriteLine($"Hash: {hash}");
Console.WriteLine();
Console.WriteLine("============================================");
Console.WriteLine("SQL UPDATE STATEMENT:");
Console.WriteLine("============================================");
Console.WriteLine($"UPDATE Users SET PasswordHash = '{hash}', UpdatedAt = GETUTCDATE() WHERE Email = 'admin@khidmah.com' AND IsDeleted = 0;");
Console.WriteLine();

// Verify
bool isValid = BCrypt.Net.BCrypt.Verify(password, hash);
Console.WriteLine($"Verification: {(isValid ? "✓ SUCCESS" : "✗ FAILED")}");
Console.WriteLine();

