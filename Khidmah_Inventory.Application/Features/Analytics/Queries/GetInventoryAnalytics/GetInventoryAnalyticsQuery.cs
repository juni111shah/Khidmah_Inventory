using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Analytics.Models;

namespace Khidmah_Inventory.Application.Features.Analytics.Queries.GetInventoryAnalytics;

public class GetInventoryAnalyticsQuery : IRequest<Result<InventoryAnalyticsDto>>
{
    public Guid? WarehouseId { get; set; }
    public Guid? CategoryId { get; set; }
}

