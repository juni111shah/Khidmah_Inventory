using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapRack;

public class DeleteMapRackCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
