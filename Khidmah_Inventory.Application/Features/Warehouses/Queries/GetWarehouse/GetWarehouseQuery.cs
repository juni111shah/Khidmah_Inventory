using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Warehouses.Models;

namespace Khidmah_Inventory.Application.Features.Warehouses.Queries.GetWarehouse;

public class GetWarehouseQuery : IRequest<Result<WarehouseDto>>
{
    public Guid Id { get; set; }
}

