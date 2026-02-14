using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// Registered webhook URL for event notifications (order created, sale completed, stock low, approval done).
/// </summary>
public class Webhook : BaseEntity
{
    public string Name { get; private set; } = string.Empty;
    public string Url { get; private set; } = string.Empty;
    /// <summary>Optional secret for signing payload (e.g. HMAC).</summary>
    public string? Secret { get; private set; }
    /// <summary>Comma-separated event names: OrderCreated, SaleCompleted, StockLow, ApprovalDone.</summary>
    public string Events { get; private set; } = string.Empty;
    public bool IsActive { get; private set; } = true;
    public string? Description { get; private set; }

    private Webhook() { }

    public Webhook(Guid companyId, string name, string url, string events, string? secret = null, string? description = null, Guid? createdBy = null)
        : base(companyId, createdBy)
    {
        Name = name;
        Url = url;
        Events = events;
        Secret = secret;
        Description = description;
    }

    public void Update(string name, string url, string events, string? secret, string? description, bool isActive, Guid? updatedBy = null)
    {
        Name = name;
        Url = url;
        Events = events;
        Secret = secret;
        Description = description;
        IsActive = isActive;
        UpdateAuditInfo(updatedBy);
    }

    public void SetActive(bool active, Guid? updatedBy = null)
    {
        IsActive = active;
        UpdateAuditInfo(updatedBy);
    }
}
