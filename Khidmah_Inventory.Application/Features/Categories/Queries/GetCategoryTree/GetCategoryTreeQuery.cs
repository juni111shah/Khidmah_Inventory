using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Queries.GetCategoryTree;

public class GetCategoryTreeQuery : IRequest<Result<List<CategoryTreeDto>>>
{
}

