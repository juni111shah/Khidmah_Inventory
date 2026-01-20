using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Suppliers.Models;

namespace Khidmah_Inventory.Application.Features.Suppliers.Commands.UpdateSupplier;

public class UpdateSupplierCommandHandler : IRequestHandler<UpdateSupplierCommand, Result<SupplierDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateSupplierCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<SupplierDto>> Handle(UpdateSupplierCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<SupplierDto>.Failure("Company context is required");

        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == companyId.Value && !s.IsDeleted, cancellationToken);

        if (supplier == null)
            return Result<SupplierDto>.Failure("Supplier not found.");

        if (!string.IsNullOrWhiteSpace(request.Code) && supplier.Code != request.Code)
        {
            var existing = await _context.Suppliers
                .FirstOrDefaultAsync(s => s.CompanyId == companyId.Value && s.Code == request.Code && s.Id != request.Id && !s.IsDeleted, cancellationToken);
            if (existing != null)
                return Result<SupplierDto>.Failure("A supplier with this code already exists.");
        }

        supplier.Update(request.Name, request.Code, request.ContactPerson, request.Email, request.PhoneNumber,
            request.Address, request.City, request.State, request.Country, request.PostalCode, request.TaxId,
            request.PaymentTerms, request.CreditLimit, _currentUser.UserId);

        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(supplier.Id, companyId.Value, cancellationToken);
        return Result<SupplierDto>.Success(dto);
    }

    private async Task<SupplierDto> MapToDtoAsync(Guid supplierId, Guid companyId, CancellationToken cancellationToken)
    {
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == supplierId && s.CompanyId == companyId, cancellationToken);

        if (supplier == null)
            throw new InvalidOperationException("Supplier not found");

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

