using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteWarehouseMap;

public class DeleteWarehouseMapCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
