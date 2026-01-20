using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.TransferStock;

public class TransferStockCommandHandler : IRequestHandler<TransferStockCommand, Result<List<StockTransactionDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public TransferStockCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<StockTransactionDto>>> Handle(TransferStockCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<StockTransactionDto>>.Failure("Company context is required");

        // Validate product
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        if (product == null)
            return Result<List<StockTransactionDto>>.Failure("Product not found.");

        // Validate warehouses
        var fromWarehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.FromWarehouseId && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);

        if (fromWarehouse == null)
            return Result<List<StockTransactionDto>>.Failure("Source warehouse not found.");

        var toWarehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.ToWarehouseId && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);

        if (toWarehouse == null)
            return Result<List<StockTransactionDto>>.Failure("Destination warehouse not found.");

        // Get source stock level
        var fromStockLevel = await _context.StockLevels
            .FirstOrDefaultAsync(sl => sl.ProductId == request.ProductId && 
                sl.WarehouseId == request.FromWarehouseId && 
                sl.CompanyId == companyId.Value, cancellationToken);

        if (fromStockLevel == null || fromStockLevel.Quantity < request.Quantity)
            return Result<List<StockTransactionDto>>.Failure("Insufficient stock in source warehouse.");

        // Get or create destination stock level
        var toStockLevel = await _context.StockLevels
            .FirstOrDefaultAsync(sl => sl.ProductId == request.ProductId && 
                sl.WarehouseId == request.ToWarehouseId && 
                sl.CompanyId == companyId.Value, cancellationToken);

        if (toStockLevel == null)
        {
            toStockLevel = new StockLevel(companyId.Value, request.ProductId, request.ToWarehouseId, 0, _currentUser.UserId);
            _context.StockLevels.Add(toStockLevel);
        }

        // Get average cost from source
        var averageCost = fromStockLevel.AverageCost ?? product.PurchasePrice;

        // Create StockOut transaction from source
        var stockOut = new StockTransaction(
            companyId.Value,
            request.ProductId,
            request.FromWarehouseId,
            "Transfer",
            request.Quantity,
            averageCost,
            _currentUser.UserId);

        stockOut.SetReference("Transfer", null, $"TRF-{DateTime.UtcNow:yyyyMMddHHmmss}");
        if (!string.IsNullOrWhiteSpace(request.BatchNumber) || request.ExpiryDate.HasValue)
            stockOut.SetBatchInfo(request.BatchNumber, request.ExpiryDate);

        var newFromBalance = fromStockLevel.Quantity - request.Quantity;
        stockOut.SetBalanceAfter(newFromBalance);

        // Create StockIn transaction to destination
        var stockIn = new StockTransaction(
            companyId.Value,
            request.ProductId,
            request.ToWarehouseId,
            "StockIn",
            request.Quantity,
            averageCost,
            _currentUser.UserId);

        stockIn.SetReference("Transfer", stockOut.Id, $"TRF-{DateTime.UtcNow:yyyyMMddHHmmss}");
        if (!string.IsNullOrWhiteSpace(request.BatchNumber) || request.ExpiryDate.HasValue)
            stockIn.SetBatchInfo(request.BatchNumber, request.ExpiryDate);

        var newToBalance = toStockLevel.Quantity + request.Quantity;
        stockIn.SetBalanceAfter(newToBalance);

        // Update stock levels
        fromStockLevel.AdjustQuantity(-request.Quantity);
        toStockLevel.AdjustQuantity(request.Quantity, averageCost);

        _context.StockTransactions.Add(stockOut);
        _context.StockTransactions.Add(stockIn);

        await _context.SaveChangesAsync(cancellationToken);

        // Map to DTOs
        var transactions = await _context.StockTransactions
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .Where(t => (t.Id == stockOut.Id || t.Id == stockIn.Id) && t.CompanyId == companyId.Value)
            .ToListAsync(cancellationToken);

        var transactionDtos = transactions.Select(t => new StockTransactionDto
        {
            Id = t.Id,
            ProductId = t.ProductId,
            ProductName = t.Product.Name,
            ProductSKU = t.Product.SKU,
            WarehouseId = t.WarehouseId,
            WarehouseName = t.Warehouse.Name,
            TransactionType = t.TransactionType,
            Quantity = t.Quantity,
            UnitCost = t.UnitCost,
            TotalCost = t.TotalCost,
            ReferenceNumber = t.ReferenceNumber,
            ReferenceType = t.ReferenceType,
            ReferenceId = t.ReferenceId,
            BatchNumber = t.BatchNumber,
            ExpiryDate = t.ExpiryDate,
            Notes = t.Notes,
            TransactionDate = t.TransactionDate,
            BalanceAfter = t.BalanceAfter,
            CreatedAt = t.CreatedAt
        }).ToList();

        return Result<List<StockTransactionDto>>.Success(transactionDtos);
    }
}

