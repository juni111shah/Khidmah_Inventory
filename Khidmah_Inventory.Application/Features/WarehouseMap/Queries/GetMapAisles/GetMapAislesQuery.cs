using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapAisles;

public class GetMapAislesQuery : IRequest<Result<List<MapAisleDto>>>
{
    public Guid MapZoneId { get; set; }
}
