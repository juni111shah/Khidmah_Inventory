using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Suppliers.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Suppliers.Commands.CreateSupplier;

public class CreateSupplierCommandHandler : IRequestHandler<CreateSupplierCommand, Result<SupplierDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateSupplierCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<SupplierDto>> Handle(CreateSupplierCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<SupplierDto>.Failure("Company context is required");
        }

        // Check if code is unique (if provided)
        if (!string.IsNullOrWhiteSpace(request.Code))
        {
            var existingSupplier = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.CompanyId == companyId.Value && s.Code == request.Code && !s.IsDeleted, cancellationToken);

            if (existingSupplier != null)
            {
                return Result<SupplierDto>.Failure("A supplier with this code already exists.");
            }
        }

        var supplier = new Supplier(companyId.Value, request.Name, request.Code, _currentUser.UserId);

        supplier.Update(
            request.Name,
            request.Code,
            request.ContactPerson,
            request.Email,
            request.PhoneNumber,
            request.Address,
            request.City,
            request.State,
            request.Country,
            request.PostalCode,
            request.TaxId,
            request.PaymentTerms,
            request.CreditLimit,
            _currentUser.UserId);

        _context.Suppliers.Add(supplier);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(supplier.Id, companyId.Value, cancellationToken);
        return Result<SupplierDto>.Success(dto);
    }

    private async Task<SupplierDto> MapToDtoAsync(Guid supplierId, Guid companyId, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == supplierId && s.CompanyId == companyId, cancellationToken);

        if (supplier == null)
        {
            throw new InvalidOperationException("Supplier not found after creation");
        }

        return new SupplierDto
        {
            Id = supplier.Id,
            Name = supplier.Name,
            Code = supplier.Code,
            ContactPerson = supplier.ContactPerson,
            Email = supplier.Email,
            PhoneNumber = supplier.PhoneNumber,
            Address = supplier.Address,
            City = supplier.City,
            State = supplier.State,
            Country = supplier.Country,
            PostalCode = supplier.PostalCode,
            TaxId = supplier.TaxId,
            PaymentTerms = supplier.PaymentTerms,
            CreditLimit = supplier.CreditLimit,
            Balance = supplier.Balance,
            IsActive = supplier.IsActive,
            CreatedAt = supplier.CreatedAt,
            UpdatedAt = supplier.UpdatedAt
        };
    }
}

