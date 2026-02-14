using System.Security.Cryptography;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;
using Khidmah_Inventory.Infrastructure.Data;

namespace Khidmah_Inventory.Infrastructure.Services;

public class ApiKeyService : IApiKeyService
{
    private const string KeyPrefix = "kh_";
    private const int PrefixLength = 8;
    private readonly ApplicationDbContext _context;

    public ApiKeyService(ApplicationDbContext context)
    {
        _context = context;
    }

    public (string KeyValue, string KeyPrefix, string KeyHash) GenerateKey()
    {
        var bytes = new byte[32];
        using (var rng = RandomNumberGenerator.Create())
            rng.GetBytes(bytes);
        var keyValue = KeyPrefix + Convert.ToBase64String(bytes).Replace("+", "").Replace("/", "").TrimEnd('=')[..(24)];
        var prefix = keyValue.Length >= PrefixLength ? keyValue[..PrefixLength] : keyValue;
        var hash = HashKey(keyValue);
        return (keyValue, prefix, hash);
    }

    public string HashKey(string rawKey)
    {
        var bytes = Encoding.UTF8.GetBytes(rawKey);
        var hash = SHA256.HashData(bytes);
        return Convert.ToHexString(hash).ToLowerInvariant();
    }

    public async Task<ApiKey?> ValidateKeyAsync(string rawKey, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(rawKey)) return null;
        var prefix = rawKey.Length >= PrefixLength ? rawKey[..PrefixLength] : rawKey;
        var hash = HashKey(rawKey);

        var key = await _context.ApiKeys
            .AsNoTracking()
            .FirstOrDefaultAsync(k => k.KeyPrefix == prefix && !k.IsDeleted, cancellationToken);
        if (key == null) return null;
        if (!key.IsActive || (key.ExpiresAt.HasValue && key.ExpiresAt.Value < DateTime.UtcNow))
            return null;
        if (key.KeyHash != hash) return null;
        return key;
    }
}
