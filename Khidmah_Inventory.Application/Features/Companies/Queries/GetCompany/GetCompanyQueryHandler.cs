using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Companies.Models;

namespace Khidmah_Inventory.Application.Features.Companies.Queries.GetCompany;

public class GetCompanyQueryHandler : IRequestHandler<GetCompanyQuery, Result<CompanyDto>>
{
    private readonly IApplicationDbContext _context;

    public GetCompanyQueryHandler(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<Result<CompanyDto>> Handle(GetCompanyQuery request, CancellationToken cancellationToken)
    {
        var company = await _context.Companies
            .FirstOrDefaultAsync(c => c.Id == request.Id, cancellationToken);

        if (company == null)
        {
            return Result<CompanyDto>.Failure("Company not found.");
        }

        var dto = new CompanyDto
        {
            Id = company.Id,
            Name = company.Name,
            LegalName = company.LegalName,
            TaxId = company.TaxId,
            RegistrationNumber = company.RegistrationNumber,
            Email = company.Email,
            PhoneNumber = company.PhoneNumber,
            Address = company.Address,
            City = company.City,
            State = company.State,
            Country = company.Country,
            PostalCode = company.PostalCode,
            LogoUrl = company.LogoUrl,
            Currency = company.Currency,
            TimeZone = company.TimeZone,
            IsActive = company.IsActive,
            SubscriptionExpiresAt = company.SubscriptionExpiresAt,
            SubscriptionPlan = company.SubscriptionPlan,
            CreatedAt = company.CreatedAt,
            UpdatedAt = company.UpdatedAt
        };

        return Result<CompanyDto>.Success(dto);
    }
}
