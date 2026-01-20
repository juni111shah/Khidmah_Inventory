using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;

namespace Khidmah_Inventory.Application.Features.Inventory.Queries.GetStockLevelsList;

public class GetStockLevelsListQuery : IRequest<Result<PagedResult<StockLevelDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? WarehouseId { get; set; }
    public bool? LowStockOnly { get; set; }
}

