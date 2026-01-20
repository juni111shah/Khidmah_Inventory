using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Customers.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Customers.Commands.CreateCustomer;

public class CreateCustomerCommandHandler : IRequestHandler<CreateCustomerCommand, Result<CustomerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateCustomerCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CustomerDto>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CustomerDto>.Failure("Company context is required");

        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var existing = await _context.Customers
                .FirstOrDefaultAsync(c => c.CompanyId == companyId.Value && c.Code == request.Code && !c.IsDeleted, cancellationToken);
            if (existing != null)
                return Result<CustomerDto>.Failure("A customer with this code already exists.");
        }

        var customer = new Customer(companyId.Value, request.Name, request.Code, _currentUser.UserId);
        customer.Update(request.Name, request.Code, request.ContactPerson, request.Email, request.PhoneNumber,
            request.Address, request.City, request.State, request.Country, request.PostalCode, request.TaxId,
            request.PaymentTerms, request.CreditLimit, _currentUser.UserId);

        _context.Customers.Add(customer);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(customer.Id, companyId.Value, cancellationToken);
        return Result<CustomerDto>.Success(dto);
    }

    private async Task<CustomerDto> MapToDtoAsync(Guid customerId, Guid companyId, CancellationToken cancellationToken)
    {
        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == customerId && c.CompanyId == companyId, cancellationToken);

        if (customer == null)
            throw new InvalidOperationException("Customer not found after creation");

        return new CustomerDto
        {
            Id = customer.Id,
            Name = customer.Name,
            Code = customer.Code,
            ContactPerson = customer.ContactPerson,
            Email = customer.Email,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            City = customer.City,
            State = customer.State,
            Country = customer.Country,
            PostalCode = customer.PostalCode,
            TaxId = customer.TaxId,
            PaymentTerms = customer.PaymentTerms,
            CreditLimit = customer.CreditLimit,
            Balance = customer.Balance,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}

