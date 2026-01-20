using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Collaboration.Models;

namespace Khidmah_Inventory.Application.Features.Collaboration.Queries.GetComments;

public class GetCommentsQuery : IRequest<Result<List<CommentDto>>>
{
    public string EntityType { get; set; } = string.Empty;
    public Guid EntityId { get; set; }
}

