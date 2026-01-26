using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Brands.Models;

namespace Khidmah_Inventory.Application.Features.Brands.Queries.GetBrand;

public class GetBrandQuery : IRequest<Result<BrandDto>>
{
    public Guid Id { get; set; }
}