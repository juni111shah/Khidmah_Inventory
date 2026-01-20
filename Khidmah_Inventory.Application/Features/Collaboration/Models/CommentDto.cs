namespace Khidmah_Inventory.Application.Features.Collaboration.Models;

public class CommentDto
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
    public bool IsEdited { get; set; }
    public DateTime? EditedAt { get; set; }
    public Guid? UserId { get; set; }
    public string? UserName { get; set; }
    public DateTime CreatedAt { get; set; }
    public string TimeAgo { get; set; } = string.Empty;
    public List<CommentDto> Replies { get; set; } = new();
}

