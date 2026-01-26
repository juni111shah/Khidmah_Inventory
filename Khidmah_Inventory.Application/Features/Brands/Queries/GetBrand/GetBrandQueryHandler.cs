using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Brands.Models;

namespace Khidmah_Inventory.Application.Features.Brands.Queries.GetBrand;

public class GetBrandQueryHandler : IRequestHandler<GetBrandQuery, Result<BrandDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetBrandQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<BrandDto>> Handle(GetBrandQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<BrandDto>.Failure("Company context is required");

        var brand = await _context.Brands
            .Where(b => b.Id == request.Id && b.CompanyId == companyId.Value)
            .Select(b => new BrandDto
            {
                Id = b.Id,
                Name = b.Name,
                Description = b.Description,
                LogoUrl = b.LogoUrl,
                Website = b.Website,
                CreatedAt = b.CreatedAt,
                UpdatedAt = b.UpdatedAt
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (brand == null)
            return Result<BrandDto>.Failure("Brand not found");

        return Result<BrandDto>.Success(brand);
    }
}