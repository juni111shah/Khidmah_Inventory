using Khidmah_Inventory.Domain.Common;

namespace Khidmah_Inventory.Domain.Entities;

public class Comment : Entity
{
    public string EntityType { get; private set; } = string.Empty; // Product, SalesOrder, PurchaseOrder, etc.
    public Guid EntityId { get; private set; }
    public string Content { get; private set; } = string.Empty;
    public Guid? ParentCommentId { get; private set; } // For threaded comments
    public bool IsEdited { get; private set; } = false;
    public DateTime? EditedAt { get; private set; }
    public Guid? UserId { get; private set; }
    public string? UserName { get; private set; }

    // Navigation properties
    public virtual User? User { get; private set; }
    public virtual Comment? ParentComment { get; private set; }
    public virtual ICollection<Comment> Replies { get; private set; } = new List<Comment>();

    private Comment() { }

    public Comment(
        Guid companyId,
        string entityType,
        Guid entityId,
        string content,
        Guid? userId = null,
        string? userName = null,
        Guid? parentCommentId = null) : base(companyId, userId)
    {
        EntityType = entityType;
        EntityId = entityId;
        Content = content;
        UserId = userId;
        UserName = userName;
        ParentCommentId = parentCommentId;
    }

    public void Update(string content, Guid? updatedBy = null)
    {
        Content = content;
        IsEdited = true;
        EditedAt = DateTime.UtcNow;
        UpdateAuditInfo(updatedBy);
    }
}

