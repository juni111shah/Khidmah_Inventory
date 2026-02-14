using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

/// <summary>
/// User/company notification. UserId null = broadcast to all company users.
/// </summary>
public class Notification : BaseEntity
{
    public Guid? UserId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public string Type { get; private set; } = "Info"; // Info, Success, Warning, Error
    public string? EntityType { get; private set; }
    public Guid? EntityId { get; private set; }
    public bool IsRead { get; private set; }

    public virtual User? User { get; private set; }

    private Notification() { }

    public Notification(
        Guid companyId,
        string title,
        string message,
        string type,
        Guid? userId = null,
        string? entityType = null,
        Guid? entityId = null,
        Guid? createdBy = null) : base(companyId, createdBy)
    {
        Title = title;
        Message = message;
        Type = type;
        UserId = userId;
        EntityType = entityType;
        EntityId = entityId;
    }

    public void MarkAsRead()
    {
        IsRead = true;
    }
}
