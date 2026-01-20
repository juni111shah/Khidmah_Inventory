using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class ActivityLog : Entity
{
    public string EntityType { get; private set; } = string.Empty; // Product, SalesOrder, PurchaseOrder, etc.
    public Guid EntityId { get; private set; }
    public string Action { get; private set; } = string.Empty; // Created, Updated, Deleted, StatusChanged, etc.
    public string Description { get; private set; } = string.Empty;
    public string? OldValues { get; private set; } // JSON string of old values
    public string? NewValues { get; private set; } // JSON string of new values
    public Guid? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string? IpAddress { get; private set; }

    // Navigation properties
    public virtual User? User { get; private set; }

    private ActivityLog() { }

    public ActivityLog(
        Guid companyId,
        string entityType,
        Guid entityId,
        string action,
        string description,
        Guid? userId = null,
        string? userName = null,
        string? ipAddress = null) : base(companyId, userId)
    {
        EntityType = entityType;
        EntityId = entityId;
        Action = action;
        Description = description;
        UserId = userId;
        UserName = userName;
        IpAddress = ipAddress;
    }

    public void SetValues(string? oldValues, string? newValues)
    {
        OldValues = oldValues;
        NewValues = newValues;
    }
}

