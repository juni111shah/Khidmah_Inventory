using MediatR;
using Khidmah_Inventory.Application.Common.Calculations.Dto;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Kpi.Queries.GetExecutiveKpis;

public class GetExecutiveKpisQuery : IRequest<Result<ExecutiveKpisDto>>
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? CategoryId { get; set; }
    public int? DeadStockDays { get; set; }
}
