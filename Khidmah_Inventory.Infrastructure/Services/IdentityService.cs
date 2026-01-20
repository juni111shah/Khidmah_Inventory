using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Khidmah_Inventory.Application.Common.Interfaces;
using BCrypt.Net;

namespace Khidmah_Inventory.Infrastructure.Services;

public class IdentityService : IIdentityService
{
    private readonly IConfiguration _configuration;

    public IdentityService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public Task<string> GeneratePasswordHashAsync(string password)
    {
        var hash = BCrypt.Net.BCrypt.HashPassword(password, BCrypt.Net.BCrypt.GenerateSalt(12));
        return Task.FromResult(hash);
    }

    public Task<bool> VerifyPasswordAsync(string password, string hash)
    {
        try
        {
            var isValid = BCrypt.Net.BCrypt.Verify(password, hash);
            return Task.FromResult(isValid);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public Task<string> GenerateJwtTokenAsync(
        Guid userId,
        string email,
        Guid companyId,
        List<string> roles,
        List<string> permissions)
    {
        var jwtSettings = _configuration.GetSection("JwtSettings");
        var secretKey = jwtSettings["SecretKey"] ?? "YourSuperSecretKeyThatShouldBeAtLeast32CharactersLong!";
        var issuer = jwtSettings["Issuer"] ?? "KhidmahInventory";
        var audience = jwtSettings["Audience"] ?? "KhidmahInventory";
        var expiryMinutes = int.Parse(jwtSettings["ExpiryMinutes"] ?? "1440"); // 24 hours

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Email, email),
            new Claim("CompanyId", companyId.ToString()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        // Add roles
        foreach (var role in roles)
        {
            claims.Add(new Claim(ClaimTypes.Role, role));
        }

        // Add permissions
        foreach (var permission in permissions)
        {
            claims.Add(new Claim("Permission", permission));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));
        var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(expiryMinutes),
            signingCredentials: credentials);

        return Task.FromResult(new JwtSecurityTokenHandler().WriteToken(token));
    }

    public Task<string> GenerateRefreshTokenAsync()
    {
        var randomNumber = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomNumber);
        return Task.FromResult(Convert.ToBase64String(randomNumber));
    }

    public Task<bool> ValidateRefreshTokenAsync(string token, string storedToken, DateTime? expiryTime)
    {
        if (string.IsNullOrEmpty(token) || string.IsNullOrEmpty(storedToken))
            return Task.FromResult(false);

        if (expiryTime.HasValue && expiryTime.Value < DateTime.UtcNow)
            return Task.FromResult(false);

        return Task.FromResult(token == storedToken);
    }
}

