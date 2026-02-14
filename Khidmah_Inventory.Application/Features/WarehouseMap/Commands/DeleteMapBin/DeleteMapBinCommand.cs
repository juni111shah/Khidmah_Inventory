using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapBin;

public class DeleteMapBinCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
