using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.HandsFree.Models;

namespace Khidmah_Inventory.Application.Features.HandsFree.Queries.GetHandsFreeSessions;

public class GetHandsFreeSessionsQuery : IRequest<Result<List<HandsFreeSupervisorSessionDto>>>
{
    /// <summary>
    /// Consider sessions active when last activity is within this many minutes.
    /// </summary>
    public int ActiveWithinMinutes { get; set; } = 60;
}
