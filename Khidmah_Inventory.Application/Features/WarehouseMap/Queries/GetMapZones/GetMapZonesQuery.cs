using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapZones;

public class GetMapZonesQuery : IRequest<Result<List<MapZoneDto>>>
{
    public Guid WarehouseMapId { get; set; }
}
