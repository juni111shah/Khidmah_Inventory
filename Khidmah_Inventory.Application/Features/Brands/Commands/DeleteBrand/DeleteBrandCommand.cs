using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Brands.Commands.DeleteBrand;

public class DeleteBrandCommand : IRequest<Result<Unit>>
{
    public Guid Id { get; set; }
}