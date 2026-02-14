using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Common.Interfaces;

/// <summary>
/// API key validation and creation (hashing). Used by API key auth middleware and API key CRUD.
/// </summary>
public interface IApiKeyService
{
    /// <summary>
    /// Generate a new key value (e.g. "kh_live_xxxx") and return prefix + hash for storage.
    /// </summary>
    (string KeyValue, string KeyPrefix, string KeyHash) GenerateKey();

    /// <summary>
    /// Validate raw key from header: lookup by prefix, verify hash, check active and expiry. Returns the entity or null.
    /// </summary>
    Task<ApiKey?> ValidateKeyAsync(string rawKey, CancellationToken cancellationToken = default);

    /// <summary>
    /// Hash a key for comparison (same algorithm as GenerateKey).
    /// </summary>
    string HashKey(string rawKey);
}
