using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Warehouses.Commands.DeleteWarehouse;

public class DeleteWarehouseCommandHandler : IRequestHandler<DeleteWarehouseCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteWarehouseCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteWarehouseCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result.Failure("Company context is required");
        }

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.Id && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);

        if (warehouse == null)
        {
            return Result.Failure("Warehouse not found.");
        }

        // Check if warehouse has zones
        var hasZones = await _context.WarehouseZones
            .AnyAsync(z => z.WarehouseId == request.Id && z.CompanyId == companyId.Value && !z.IsDeleted, cancellationToken);

        if (hasZones)
        {
            return Result.Failure("Cannot delete warehouse with zones. Please delete or reassign zones first.");
        }

        // Check if warehouse has stock transactions
        var hasStockTransactions = await _context.StockTransactions
            .AnyAsync(st => st.WarehouseId == request.Id && st.CompanyId == companyId.Value, cancellationToken);

        if (hasStockTransactions)
        {
            return Result.Failure("Cannot delete warehouse with stock transactions. Please deactivate the warehouse instead.");
        }

        warehouse.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}

