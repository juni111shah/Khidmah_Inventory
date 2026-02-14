using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapBin;

public class UpdateMapBinCommand : IRequest<Result<MapBinDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Code { get; set; }
    public decimal X { get; set; }
    public decimal Y { get; set; }
    public int DisplayOrder { get; set; }
    public Guid? BinId { get; set; }
}
