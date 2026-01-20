using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Categories.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Commands.UpdateCategory;

public class UpdateCategoryCommand : IRequest<Result<CategoryDto>>
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Code { get; set; }
    public Guid? ParentCategoryId { get; set; }
    public int DisplayOrder { get; set; } = 0;
}

