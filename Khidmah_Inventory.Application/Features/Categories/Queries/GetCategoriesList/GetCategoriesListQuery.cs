using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Queries.GetCategoriesList;

public class GetCategoriesListQuery : IRequest<Result<PagedResult<CategoryDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public Guid? ParentCategoryId { get; set; }
}

