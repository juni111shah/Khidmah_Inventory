using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetSalesReport;

public class GetSalesReportQuery : IRequest<Result<SalesReportDto>>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? CustomerId { get; set; }
}

