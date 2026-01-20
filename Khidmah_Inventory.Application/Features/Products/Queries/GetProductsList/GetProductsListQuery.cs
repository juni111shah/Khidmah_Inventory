using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Products.Models;

namespace Khidmah_Inventory.Application.Features.Products.Queries.GetProductsList;

public class GetProductsListQuery : IRequest<Result<PagedResult<ProductDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public Guid? CategoryId { get; set; }
    public Guid? BrandId { get; set; }
    public bool? IsActive { get; set; }
}

