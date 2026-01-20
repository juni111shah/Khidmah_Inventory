using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetCustomReportsList;

public class GetCustomReportsListQuery : IRequest<Result<List<CustomReportDto>>>
{
    public string? ReportType { get; set; }
    public bool? IncludePublic { get; set; } = true;
}

