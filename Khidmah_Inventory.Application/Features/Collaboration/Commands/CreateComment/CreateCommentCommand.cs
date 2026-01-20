using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Collaboration.Models;

namespace Khidmah_Inventory.Application.Features.Collaboration.Commands.CreateComment;

public class CreateCommentCommand : IRequest<Result<CommentDto>>
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid? ParentCommentId { get; set; }
}

