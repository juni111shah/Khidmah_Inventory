using MediatR;
using Khidmah_Inventory.Application.Common.Calculations.Dto;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Kpi.Queries.GetSalesKpis;

public class GetSalesKpisQuery : IRequest<Result<SalesKpisDto>>
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? CategoryId { get; set; }
}
