namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Models;

public class WebhookDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
    public string Events { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class WebhookDeliveryLogDto
{
    public Guid Id { get; set; }
    public string EventName { get; set; } = string.Empty;
    public int HttpStatusCode { get; set; }
    public bool Success { get; set; }
    public int RetryCount { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime DeliveredAt { get; set; }
}
