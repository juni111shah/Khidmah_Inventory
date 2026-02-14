using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Log of each webhook delivery attempt (for retry and audit).
/// </summary>
public class WebhookDeliveryLog : BaseEntity
{
    public Guid WebhookId { get; private set; }
    public string EventName { get; private set; } = string.Empty;
    public string PayloadJson { get; private set; } = string.Empty;
    public int HttpStatusCode { get; private set; }
    public bool Success { get; private set; }
    public int RetryCount { get; private set; }
    public string? ResponseBody { get; private set; }
    public string? ErrorMessage { get; private set; }
    public DateTime DeliveredAt { get; private set; } = DateTime.UtcNow;

    private WebhookDeliveryLog() { }

    public WebhookDeliveryLog(Guid companyId, Guid webhookId, string eventName, string payloadJson, int httpStatusCode, bool success, int retryCount, string? responseBody = null, string? errorMessage = null)
        : base(companyId)
    {
        WebhookId = webhookId;
        EventName = eventName;
        PayloadJson = payloadJson;
        HttpStatusCode = httpStatusCode;
        Success = success;
        RetryCount = retryCount;
        ResponseBody = responseBody;
        ErrorMessage = errorMessage;
    }
}
