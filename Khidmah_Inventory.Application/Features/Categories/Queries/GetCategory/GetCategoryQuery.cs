using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Queries.GetCategory;

public class GetCategoryQuery : IRequest<Result<CategoryDto>>
{
    public Guid Id { get; set; }
}

