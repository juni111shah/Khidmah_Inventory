namespace Khidmah_Inventory.API.Models;

/// <summary>
/// Envelope for all real-time operations events. Sent to clients with every event.
/// </summary>
public class OperationsEventPayload
{
    public Guid CompanyId { get; set; }
    public Guid? EntityId { get; set; }
    public string? EntityType { get; set; }
    public object? Payload { get; set; }
    public DateTime TimestampUtc { get; set; } = DateTime.UtcNow;
}
