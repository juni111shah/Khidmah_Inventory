using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;

namespace Khidmah_Inventory.Application.Features.Inventory.Queries.GetBatchesList;

public class GetBatchesListQuery : IRequest<Result<PagedResult<BatchDto>>>
{
    public FilterRequest FilterRequest { get; set; } = new();
    public Guid? ProductId { get; set; }
    public Guid? WarehouseId { get; set; }
    public bool? ExpiringSoon { get; set; }
    public bool? Expired { get; set; }
    public bool? Recalled { get; set; }
}

