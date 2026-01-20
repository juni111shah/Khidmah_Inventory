using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Suppliers.Models;

namespace Khidmah_Inventory.Application.Features.Suppliers.Queries.GetSuppliersList;

public class GetSuppliersListQuery : IRequest<Result<PagedResult<SupplierDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public bool? IsActive { get; set; }
}

