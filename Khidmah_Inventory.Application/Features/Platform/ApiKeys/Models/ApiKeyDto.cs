namespace Khidmah_Inventory.Application.Features.Platform.ApiKeys.Models;

public class ApiKeyDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string KeyPrefix { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
    public bool IsActive { get; set; }
    public DateTime? LastUsedAt { get; set; }
    public long RequestCount { get; set; }
    public long ErrorCount { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class ApiKeyUsageDto
{
    public Guid ApiKeyId { get; set; }
    public string Name { get; set; } = string.Empty;
    public long TotalCalls { get; set; }
    public long ErrorCalls { get; set; }
    public DateTime? LastAccessAt { get; set; }
    public List<ApiKeyUsageLogDto>? RecentLogs { get; set; }
}

public class ApiKeyUsageLogDto
{
    public string Method { get; set; } = string.Empty;
    public string Path { get; set; } = string.Empty;
    public int StatusCode { get; set; }
    public bool Success { get; set; }
    public long ElapsedMs { get; set; }
    public DateTime CreatedAt { get; set; }
}
