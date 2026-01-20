using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.RecallBatch;

public class RecallBatchCommandHandler : IRequestHandler<RecallBatchCommand, Result<BatchDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public RecallBatchCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<BatchDto>> Handle(RecallBatchCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<BatchDto>.Failure("Company context is required");

        var batch = await _context.Batches
            .Include(b => b.Product)
            .Include(b => b.Warehouse)
            .FirstOrDefaultAsync(b => b.Id == request.BatchId && b.CompanyId == companyId.Value && !b.IsDeleted, cancellationToken);

        if (batch == null)
            return Result<BatchDto>.Failure("Batch not found.");

        batch.Recall(request.Reason, _currentUser.UserId);

        // Also recall all serial numbers in this batch
        var serialNumbers = await _context.SerialNumbers
            .Where(s => s.BatchId == request.BatchId && s.CompanyId == companyId.Value && !s.IsDeleted)
            .ToListAsync(cancellationToken);

        foreach (var serial in serialNumbers)
        {
            serial.Recall($"Batch recalled: {request.Reason}", _currentUser.UserId);
        }

        await _context.SaveChangesAsync(cancellationToken);

        var dto = new BatchDto
        {
            Id = batch.Id,
            ProductId = batch.ProductId,
            ProductName = batch.Product.Name,
            ProductSKU = batch.Product.SKU,
            WarehouseId = batch.WarehouseId,
            WarehouseName = batch.Warehouse.Name,
            BatchNumber = batch.BatchNumber,
            LotNumber = batch.LotNumber,
            ManufactureDate = batch.ManufactureDate,
            ExpiryDate = batch.ExpiryDate,
            Quantity = batch.Quantity,
            UnitCost = batch.UnitCost,
            SupplierName = batch.SupplierName,
            SupplierBatchNumber = batch.SupplierBatchNumber,
            Notes = batch.Notes,
            IsRecalled = batch.IsRecalled,
            RecallDate = batch.RecallDate,
            RecallReason = batch.RecallReason,
            IsExpired = batch.IsExpired,
            IsExpiringSoon = batch.IsExpiringSoon,
            DaysUntilExpiry = batch.DaysUntilExpiry,
            CreatedAt = batch.CreatedAt
        };

        return Result<BatchDto>.Success(dto);
    }
}

