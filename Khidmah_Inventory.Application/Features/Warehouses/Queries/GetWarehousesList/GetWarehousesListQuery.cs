using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Warehouses.Models;

namespace Khidmah_Inventory.Application.Features.Warehouses.Queries.GetWarehousesList;

public class GetWarehousesListQuery : IRequest<Result<PagedResult<WarehouseDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public bool? IsActive { get; set; }
}

