namespace Khidmah_Inventory.Application.Common.Interfaces;

public interface IIdentityService
{
    Task<string> GeneratePasswordHashAsync(string password);
    Task<bool> VerifyPasswordAsync(string password, string hash);
    Task<string> GenerateJwtTokenAsync(Guid userId, string email, Guid companyId, List<string> roles, List<string> permissions);
    Task<string> GenerateRefreshTokenAsync();
    Task<bool> ValidateRefreshTokenAsync(string token, string storedToken, DateTime? expiryTime);
}

