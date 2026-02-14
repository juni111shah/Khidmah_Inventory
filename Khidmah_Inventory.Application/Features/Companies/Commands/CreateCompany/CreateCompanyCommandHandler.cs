using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Companies.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Companies.Commands.CreateCompany;

public class CreateCompanyCommandHandler : IRequestHandler<CreateCompanyCommand, Result<CompanyDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCompanyCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CompanyDto>> Handle(CreateCompanyCommand request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(request.Name))
        {
            return Result<CompanyDto>.Failure("Company name is required.");
        }

        var id = Guid.NewGuid();
        var company = new Company(id, request.Name.Trim(), request.Email?.Trim(), _currentUser.UserId);

        company.Update(
            request.Name.Trim(),
            request.LegalName?.Trim(),
            request.TaxId?.Trim(),
            request.RegistrationNumber?.Trim(),
            request.Email?.Trim(),
            request.PhoneNumber?.Trim(),
            request.Address?.Trim(),
            request.City?.Trim(),
            request.State?.Trim(),
            request.Country?.Trim(),
            request.PostalCode?.Trim(),
            request.Currency?.Trim(),
            request.TimeZone?.Trim(),
            _currentUser.UserId);

        _context.Companies.Add(company);
        await _context.SaveChangesAsync(cancellationToken);

        // Use the same tracked entity; avoid re-query (query filters may exclude newly created tenant root)
        var dto = MapToDto(company);
        return Result<CompanyDto>.Success(dto);
    }

    private static CompanyDto MapToDto(Company c)
    {
        return new CompanyDto
        {
            Id = c.Id,
            Name = c.Name,
            LegalName = c.LegalName,
            TaxId = c.TaxId,
            RegistrationNumber = c.RegistrationNumber,
            Email = c.Email,
            PhoneNumber = c.PhoneNumber,
            Address = c.Address,
            City = c.City,
            State = c.State,
            Country = c.Country,
            PostalCode = c.PostalCode,
            LogoUrl = c.LogoUrl,
            Currency = c.Currency,
            TimeZone = c.TimeZone,
            IsActive = c.IsActive,
            SubscriptionExpiresAt = c.SubscriptionExpiresAt,
            SubscriptionPlan = c.SubscriptionPlan,
            CreatedAt = c.CreatedAt,
            UpdatedAt = c.UpdatedAt
        };
    }
}
