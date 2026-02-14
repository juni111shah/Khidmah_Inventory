using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Companies.Models;

namespace Khidmah_Inventory.Application.Features.Companies.Commands.ActivateCompany;

public class ActivateCompanyCommandHandler : IRequestHandler<ActivateCompanyCommand, Result<CompanyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ActivateCompanyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CompanyDto>> Handle(ActivateCompanyCommand request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company == null)
        {
            return Result<CompanyDto>.Failure("Company not found.");
        }

        company.Activate(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        var updated = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (updated == null)
        {
            return Result<CompanyDto>.Failure("Company could not be retrieved after activation.");
        }

        var dto = new CompanyDto
        {
            Id = updated.Id,
            Name = updated.Name,
            LegalName = updated.LegalName,
            TaxId = updated.TaxId,
            RegistrationNumber = updated.RegistrationNumber,
            Email = updated.Email,
            PhoneNumber = updated.PhoneNumber,
            Address = updated.Address,
            City = updated.City,
            State = updated.State,
            Country = updated.Country,
            PostalCode = updated.PostalCode,
            LogoUrl = updated.LogoUrl,
            Currency = updated.Currency,
            TimeZone = updated.TimeZone,
            IsActive = updated.IsActive,
            SubscriptionExpiresAt = updated.SubscriptionExpiresAt,
            SubscriptionPlan = updated.SubscriptionPlan,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };

        return Result<CompanyDto>.Success(dto);
    }
}
