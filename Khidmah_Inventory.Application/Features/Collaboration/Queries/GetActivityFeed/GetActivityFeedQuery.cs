using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Collaboration.Models;

namespace Khidmah_Inventory.Application.Features.Collaboration.Queries.GetActivityFeed;

public class GetActivityFeedQuery : IRequest<Result<List<ActivityLogDto>>>
{
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
    public int Limit { get; set; } = 50;
}

