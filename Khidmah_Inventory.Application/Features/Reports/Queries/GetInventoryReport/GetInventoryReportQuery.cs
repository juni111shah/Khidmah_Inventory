using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetInventoryReport;

public class GetInventoryReportQuery : IRequest<Result<InventoryReportDto>>
{
    public Guid? WarehouseId { get; set; }
    public Guid? CategoryId { get; set; }
    public bool? LowStockOnly { get; set; }
}

