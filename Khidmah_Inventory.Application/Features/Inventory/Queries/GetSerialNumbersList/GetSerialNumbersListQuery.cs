using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;

namespace Khidmah_Inventory.Application.Features.Inventory.Queries.GetSerialNumbersList;

public class GetSerialNumbersListQuery : IRequest<Result<PagedResult<SerialNumberDto>>>
{
    public FilterRequest FilterRequest { get; set; } = new();
    public Guid? ProductId { get; set; }
    public Guid? WarehouseId { get; set; }
    public Guid? BatchId { get; set; }
    public string? Status { get; set; }
    public bool? ExpiringSoon { get; set; }
    public bool? Expired { get; set; }
    public bool? Recalled { get; set; }
}

