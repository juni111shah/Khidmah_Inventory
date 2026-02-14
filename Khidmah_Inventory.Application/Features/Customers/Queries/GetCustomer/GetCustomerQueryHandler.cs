using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Customers.Models;

namespace Khidmah_Inventory.Application.Features.Customers.Queries.GetCustomer;

public class GetCustomerQueryHandler : IRequestHandler<GetCustomerQuery, Result<CustomerDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetCustomerQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<CustomerDto>> Handle(GetCustomerQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<CustomerDto>.Failure("Company context is required");

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.Id && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        if (customer == null)
            return Result<CustomerDto>.Failure("Customer not found.");

        var dto = new CustomerDto
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

        return Result<CustomerDto>.Success(dto);
    }
}
