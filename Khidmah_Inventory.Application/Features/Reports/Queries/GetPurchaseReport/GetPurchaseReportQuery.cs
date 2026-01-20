using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetPurchaseReport;

public class GetPurchaseReportQuery : IRequest<Result<PurchaseReportDto>>
{
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
    public Guid? SupplierId { get; set; }
}

