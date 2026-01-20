using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.ExecuteCustomReport;

public class ExecuteCustomReportQuery : IRequest<Result<object>>
{
    public Guid ReportId { get; set; }
    public Dictionary<string, object>? Parameters { get; set; }
}

