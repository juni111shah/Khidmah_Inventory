using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetWarehouseMapById;

public class GetWarehouseMapByIdQuery : IRequest<Result<WarehouseMapTreeDto>>
{
    public Guid Id { get; set; }
}
