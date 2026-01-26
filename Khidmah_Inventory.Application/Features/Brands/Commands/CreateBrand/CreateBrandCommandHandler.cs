using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Brands.Commands.CreateBrand;

public class CreateBrandCommandHandler : IRequestHandler<CreateBrandCommand, Result<Guid>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateBrandCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<Guid>> Handle(CreateBrandCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<Guid>.Failure("Company context is required");

        var brand = new Brand(companyId.Value, request.Name, _currentUser.UserId);
        brand.Update(request.Name, request.Description, request.Website, _currentUser.UserId);

        _context.Brands.Add(brand);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<Guid>.Success(brand.Id);
    }
}