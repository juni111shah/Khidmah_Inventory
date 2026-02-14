using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Recent API key usage for dashboard (endpoint, status, time). Optional high-volume logging.
/// </summary>
public class ApiKeyUsageLog : BaseEntity
{
    public Guid ApiKeyId { get; private set; }
    public string Method { get; private set; } = string.Empty;
    public string Path { get; private set; } = string.Empty;
    public int StatusCode { get; private set; }
    public bool Success { get; private set; }
    public long ElapsedMs { get; private set; }

    private ApiKeyUsageLog() { }

    public ApiKeyUsageLog(Guid companyId, Guid apiKeyId, string method, string path, int statusCode, bool success, long elapsedMs)
        : base(companyId)
    {
        ApiKeyId = apiKeyId;
        Method = method;
        Path = path;
        StatusCode = statusCode;
        Success = success;
        ElapsedMs = elapsedMs;
    }
}
