using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapZone;

public class UpdateMapZoneCommand : IRequest<Result<MapZoneDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public int DisplayOrder { get; set; }
}
