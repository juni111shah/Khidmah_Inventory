using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// API key for external/machine access. Key itself is shown once at creation; only hash and prefix stored.
/// </summary>
public class ApiKey : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    /// <summary>First 8 chars of key for lookup (e.g. "kh_live_").</summary>
    public string KeyPrefix { get; private set; } = string.Empty;
    /// <summary>SHA256 hash of the full key.</summary>
    public string KeyHash { get; private set; } = string.Empty;
    /// <summary>Comma-separated permissions (e.g. "Products:Read,Stock:Read").</summary>
    public string Permissions { get; private set; } = string.Empty;
    public DateTime? ExpiresAt { get; private set; }
    public bool IsActive { get; private set; } = true;
    public DateTime? LastUsedAt { get; private set; }
    public long RequestCount { get; private set; }
    public long ErrorCount { get; private set; }

    private ApiKey() { }

    public ApiKey(Guid companyId, string name, string keyPrefix, string keyHash, string permissions, DateTime? expiresAt, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        Name = name;
        KeyPrefix = keyPrefix;
        KeyHash = keyHash;
        Permissions = permissions;
        ExpiresAt = expiresAt;
    }

    public void UpdateUsage(bool success)
    {
        LastUsedAt = DateTime.UtcNow;
        RequestCount++;
        if (!success) ErrorCount++;
    }

    public void Revoke(Guid? updatedBy = null)
    {
        IsActive = false;
        UpdateAuditInfo(updatedBy);
    }

    public void Update(string name, string permissions, DateTime? expiresAt, Guid? updatedBy = null)
    {
        Name = name;
        Permissions = permissions;
        ExpiresAt = expiresAt;
        UpdateAuditInfo(updatedBy);
    }
}
