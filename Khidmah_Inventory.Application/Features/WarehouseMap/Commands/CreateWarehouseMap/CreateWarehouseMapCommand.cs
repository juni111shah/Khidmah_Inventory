using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateWarehouseMap;

public class CreateWarehouseMapCommand : IRequest<Result<WarehouseMapDto>>
{
    public Guid WarehouseId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
}
