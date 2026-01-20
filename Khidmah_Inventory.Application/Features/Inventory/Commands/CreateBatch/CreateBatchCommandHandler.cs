using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.CreateBatch;

public class CreateBatchCommandHandler : IRequestHandler<CreateBatchCommand, Result<BatchDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateBatchCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<BatchDto>> Handle(CreateBatchCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<BatchDto>.Failure("Company context is required");

        // Validate product
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        if (product == null)
            return Result<BatchDto>.Failure("Product not found.");

        // Validate warehouse
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.WarehouseId && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);

        if (warehouse == null)
            return Result<BatchDto>.Failure("Warehouse not found.");

        // Check if batch number already exists
        var existingBatch = await _context.Batches
            .FirstOrDefaultAsync(b => b.CompanyId == companyId.Value && 
                b.ProductId == request.ProductId && 
                b.BatchNumber == request.BatchNumber && 
                !b.IsDeleted, cancellationToken);

        if (existingBatch != null)
            return Result<BatchDto>.Failure("Batch number already exists for this product.");

        // Create batch
        var batch = new Batch(companyId.Value, request.ProductId, request.WarehouseId, request.BatchNumber, request.Quantity, _currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(request.LotNumber))
            batch.SetLotNumber(request.LotNumber);

        if (request.ManufactureDate.HasValue)
            batch.SetManufactureDate(request.ManufactureDate);

        if (request.ExpiryDate.HasValue)
            batch.SetExpiryDate(request.ExpiryDate);

        if (request.UnitCost.HasValue)
            batch.SetCost(request.UnitCost);

        if (!string.IsNullOrWhiteSpace(request.SupplierName) || !string.IsNullOrWhiteSpace(request.SupplierBatchNumber))
            batch.SetSupplierInfo(request.SupplierName, request.SupplierBatchNumber);

        _context.Batches.Add(batch);
        await _context.SaveChangesAsync(cancellationToken);

        // Load with navigation properties for DTO mapping
        var batchWithNav = await _context.Batches
            .Include(b => b.Product)
            .Include(b => b.Warehouse)
            .FirstOrDefaultAsync(b => b.Id == batch.Id && b.CompanyId == companyId.Value, cancellationToken);

        var dto = new BatchDto
        {
            Id = batchWithNav!.Id,
            ProductId = batchWithNav.ProductId,
            ProductName = batchWithNav.Product.Name,
            ProductSKU = batchWithNav.Product.SKU,
            WarehouseId = batchWithNav.WarehouseId,
            WarehouseName = batchWithNav.Warehouse.Name,
            BatchNumber = batchWithNav.BatchNumber,
            LotNumber = batchWithNav.LotNumber,
            ManufactureDate = batchWithNav.ManufactureDate,
            ExpiryDate = batchWithNav.ExpiryDate,
            Quantity = batchWithNav.Quantity,
            UnitCost = batchWithNav.UnitCost,
            SupplierName = batchWithNav.SupplierName,
            SupplierBatchNumber = batchWithNav.SupplierBatchNumber,
            Notes = batchWithNav.Notes,
            IsRecalled = batchWithNav.IsRecalled,
            RecallDate = batchWithNav.RecallDate,
            RecallReason = batchWithNav.RecallReason,
            IsExpired = batchWithNav.IsExpired,
            IsExpiringSoon = batchWithNav.IsExpiringSoon,
            DaysUntilExpiry = batchWithNav.DaysUntilExpiry,
            CreatedAt = batchWithNav.CreatedAt
        };

        return Result<BatchDto>.Success(dto);
    }
}

