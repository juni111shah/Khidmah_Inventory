using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Dashboard.Models;

namespace Khidmah_Inventory.Application.Features.Dashboard.Queries.GetDashboardData;

public class GetDashboardDataQuery : IRequest<Result<DashboardDto>>
{
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

