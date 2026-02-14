namespace Khidmah_Inventory.Application.Common.Interfaces;

/// <summary>
/// Enqueue webhook delivery for event. Processed by background service with retry.
/// </summary>
public interface IWebhookDispatchService
{
    /// <summary>
    /// Enqueue delivery to all webhooks subscribed to the event for the company.
    /// </summary>
    Task DispatchAsync(Guid companyId, string eventName, object payload, CancellationToken cancellationToken = default);
}
