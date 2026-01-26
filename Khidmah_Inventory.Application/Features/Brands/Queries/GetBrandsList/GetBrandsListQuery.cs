using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Brands.Models;

namespace Khidmah_Inventory.Application.Features.Brands.Queries.GetBrandsList;

public class GetBrandsListQuery : IRequest<Result<PagedResult<BrandDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public string? SearchTerm { get; set; }
}