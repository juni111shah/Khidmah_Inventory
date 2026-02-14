using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetWarehouseMapsList;

public class GetWarehouseMapsListQuery : IRequest<Result<List<WarehouseMapDto>>>
{
    public Guid? WarehouseId { get; set; }
    public bool? IsActive { get; set; }
}
