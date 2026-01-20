using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Suppliers.Commands.DeleteSupplier;

public class DeleteSupplierCommandHandler : IRequestHandler<DeleteSupplierCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteSupplierCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteSupplierCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required");

        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.Id && s.CompanyId == companyId.Value && !s.IsDeleted, cancellationToken);

        if (supplier == null)
            return Result.Failure("Supplier not found.");

        var hasOrders = await _context.PurchaseOrders
            .AnyAsync(po => po.SupplierId == request.Id && po.CompanyId == companyId.Value && !po.IsDeleted, cancellationToken);

        if (hasOrders)
            return Result.Failure("Cannot delete supplier with purchase orders. Please deactivate instead.");

        supplier.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}

