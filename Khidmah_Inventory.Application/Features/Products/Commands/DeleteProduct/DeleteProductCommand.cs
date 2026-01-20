using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Products.Commands.DeleteProduct;

public class DeleteProductCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}

