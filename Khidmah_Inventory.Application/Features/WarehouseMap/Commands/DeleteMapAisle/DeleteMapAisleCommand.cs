using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapAisle;

public class DeleteMapAisleCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
