using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Categories.Commands.DeleteCategory;

public class DeleteCategoryCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

