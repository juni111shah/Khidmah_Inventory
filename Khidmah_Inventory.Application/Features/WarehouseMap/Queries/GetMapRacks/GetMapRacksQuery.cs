using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapRacks;

public class GetMapRacksQuery : IRequest<Result<List<MapRackDto>>>
{
    public Guid MapAisleId { get; set; }
}
