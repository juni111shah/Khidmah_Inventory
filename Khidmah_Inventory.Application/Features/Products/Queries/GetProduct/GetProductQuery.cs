using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Products.Models;

namespace Khidmah_Inventory.Application.Features.Products.Queries.GetProduct;

public class GetProductQuery : IRequest<Result<ProductDto>>
{
    public Guid Id { get; set; }
}

