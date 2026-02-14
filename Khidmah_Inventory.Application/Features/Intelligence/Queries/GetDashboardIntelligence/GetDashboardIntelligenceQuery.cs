using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Intelligence.Models;

namespace Khidmah_Inventory.Application.Features.Intelligence.Queries.GetDashboardIntelligence;

public class GetDashboardIntelligenceQuery : IRequest<Result<DashboardIntelligenceDto>>
{
    public int? PredictionDays { get; set; } = 7;
}
