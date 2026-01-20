using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Warehouses.Models;

namespace Khidmah_Inventory.Application.Features.Warehouses.Commands.DeactivateWarehouse;

public class DeactivateWarehouseCommand : IRequest<Result<WarehouseDto>>
{
    public Guid Id { get; set; }
}

