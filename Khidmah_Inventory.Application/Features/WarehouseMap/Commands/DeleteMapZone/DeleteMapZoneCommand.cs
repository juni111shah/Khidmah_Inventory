using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapZone;

public class DeleteMapZoneCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
