using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Warehouses.Commands.DeleteWarehouse;

public class DeleteWarehouseCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

