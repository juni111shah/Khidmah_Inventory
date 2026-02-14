namespace Khidmah_Inventory.Application.Common.Interfaces;

/// <summary>
/// Broadcasts real-time operations events to connected clients (e.g. via SignalR).
/// Implemented in the API layer; handlers use this after successful save.
/// </summary>
public interface IOperationsBroadcast
{
    /// <summary>
    /// Sends an event to all connected clients for the given company.
    /// </summary>
    /// <param name="eventName">Event name (e.g. StockChanged, OrderCreated).</param>
    /// <param name="companyId">Tenant company ID; only users in this company receive the event.</param>
    /// <param name="entityId">Optional entity ID (e.g. product, order).</param>
    /// <param name="entityType">Optional entity type (e.g. Product, SalesOrder).</param>
    /// <param name="payload">Small payload object (will be serialized).</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    Task BroadcastAsync(
        string eventName,
        Guid companyId,
        Guid? entityId,
        string? entityType,
        object? payload,
        CancellationToken cancellationToken = default);
}
