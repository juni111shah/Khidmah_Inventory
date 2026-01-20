using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Products.Models;

namespace Khidmah_Inventory.Application.Features.Products.Commands.DeactivateProduct;

public class DeactivateProductCommand : IRequest<Result<ProductDto>>
{
    public Guid Id { get; set; }
}

