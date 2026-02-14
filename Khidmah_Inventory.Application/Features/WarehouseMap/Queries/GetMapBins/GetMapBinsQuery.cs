using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapBins;

public class GetMapBinsQuery : IRequest<Result<List<MapBinDto>>>
{
    public Guid MapRackId { get; set; }
}
