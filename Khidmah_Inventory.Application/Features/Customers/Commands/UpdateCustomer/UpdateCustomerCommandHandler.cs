using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Customers.Models;

namespace Khidmah_Inventory.Application.Features.Customers.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, Result<CustomerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateCustomerCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CustomerDto>> Handle(UpdateCustomerCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CustomerDto>.Failure("Company context is required");

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        if (customer == null)
            return Result<CustomerDto>.Failure("Customer not found.");

        customer.Update(
            request.Name.Trim(),
            request.Code?.Trim(),
            request.ContactPerson?.Trim(),
            request.Email?.Trim(),
            request.PhoneNumber?.Trim(),
            request.Address?.Trim(),
            request.City?.Trim(),
            request.State?.Trim(),
            request.Country?.Trim(),
            request.PostalCode?.Trim(),
            request.TaxId?.Trim(),
            request.PaymentTerms?.Trim(),
            request.CreditLimit,
            _currentUser.UserId);

        await _context.SaveChangesAsync(cancellationToken);

        var updated = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId.Value, cancellationToken);

        if (updated == null)
            return Result<CustomerDto>.Failure("Customer could not be retrieved after update.");

        var dto = new CustomerDto
        {
            Id = updated.Id,
            Name = updated.Name,
            Code = updated.Code,
            ContactPerson = updated.ContactPerson,
            Email = updated.Email,
            PhoneNumber = updated.PhoneNumber,
            Address = updated.Address,
            City = updated.City,
            State = updated.State,
            Country = updated.Country,
            PostalCode = updated.PostalCode,
            TaxId = updated.TaxId,
            PaymentTerms = updated.PaymentTerms,
            CreditLimit = updated.CreditLimit,
            Balance = updated.Balance,
            IsActive = updated.IsActive,
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };

        return Result<CustomerDto>.Success(dto);
    }
}
