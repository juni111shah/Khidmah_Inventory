using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ScheduledReports.Queries.GetScheduledReportsList;

public class GetScheduledReportsListQuery : IRequest<Result<List<ScheduledReportDto>>>
{
    public bool? IsActive { get; set; }
}
