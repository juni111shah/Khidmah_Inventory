using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Constants;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;
using Khidmah_Inventory.Application.Features.Notifications.Commands.CreateNotification;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.CreateStockTransaction;

public class CreateStockTransactionCommandHandler : IRequestHandler<CreateStockTransactionCommand, Result<StockTransactionDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;
    private readonly IOperationsBroadcast? _broadcast;
    private readonly IAutomationExecutor? _automationExecutor;

    public CreateStockTransactionCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator,
        IOperationsBroadcast? broadcast = null,
        IAutomationExecutor? automationExecutor = null)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
        _broadcast = broadcast;
        _automationExecutor = automationExecutor;
    }

    public async Task<Result<StockTransactionDto>> Handle(CreateStockTransactionCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<StockTransactionDto>.Failure("Company context is required");
        }

        // Validate product
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        if (product == null)
        {
            return Result<StockTransactionDto>.Failure("Product not found.");
        }

        // Validate warehouse
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.WarehouseId && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);

        if (warehouse == null)
        {
            return Result<StockTransactionDto>.Failure("Warehouse not found.");
        }

        // Calculate quantity change based on transaction type
        decimal quantityChange = request.TransactionType switch
        {
            "StockIn" => request.Quantity,
            "StockOut" => -request.Quantity,
            "Adjustment" => request.Quantity, // Can be positive or negative
            "Transfer" => -request.Quantity, // Out from source warehouse
            _ => 0
        };

        // Get or create stock level
        var stockLevel = await _context.StockLevels
            .FirstOrDefaultAsync(sl => sl.ProductId == request.ProductId && sl.WarehouseId == request.WarehouseId && sl.CompanyId == companyId.Value, cancellationToken);

        if (stockLevel == null)
        {
            stockLevel = new StockLevel(companyId.Value, request.ProductId, request.WarehouseId, 0, _currentUser.UserId);
            _context.StockLevels.Add(stockLevel);
        }

        // Calculate new balance
        var newBalance = stockLevel.Quantity + quantityChange;

        // For StockOut, check if sufficient stock is available
        if (request.TransactionType == "StockOut" && newBalance < 0)
        {
            return Result<StockTransactionDto>.Failure("Insufficient stock available.");
        }

        // Create transaction
        var transaction = new StockTransaction(
            companyId.Value,
            request.ProductId,
            request.WarehouseId,
            request.TransactionType,
            request.Quantity,
            request.UnitCost,
            _currentUser.UserId);

        if (!string.IsNullOrWhiteSpace(request.ReferenceType))
        {
            transaction.SetReference(request.ReferenceType, request.ReferenceId, request.ReferenceNumber);
        }

        if (!string.IsNullOrWhiteSpace(request.BatchNumber) || request.ExpiryDate.HasValue)
        {
            transaction.SetBatchInfo(request.BatchNumber, request.ExpiryDate);
        }

        transaction.SetBalanceAfter(newBalance);

        if (request.TransactionDate.HasValue)
        {
            // Note: TransactionDate is set in constructor, we'd need to add a method to set it
        }

        _context.StockTransactions.Add(transaction);

        // Update stock level
        stockLevel.AdjustQuantity(quantityChange, request.UnitCost);

        await _context.SaveChangesAsync(cancellationToken);

        if (product.MinStockLevel.HasValue && newBalance <= product.MinStockLevel.Value)
        {
            await _mediator.Send(new CreateNotificationCommand
            {
                CompanyId = companyId.Value,
                UserId = null,
                Title = "Stock below threshold",
                Message = $"Product {product.Name} ({product.SKU}) is below minimum stock level. Current: {newBalance}, Min: {product.MinStockLevel.Value}.",
                Type = "Warning",
                EntityType = "Product",
                EntityId = product.Id
            }, cancellationToken);

            if (_automationExecutor != null)
            {
                await _automationExecutor.ExecuteStockBelowThresholdAsync(
                    companyId.Value,
                    product.Id,
                    request.WarehouseId,
                    newBalance,
                    product.MinStockLevel.Value,
                    cancellationToken);
            }
        }

        if (_broadcast != null)
        {
            await _broadcast.BroadcastAsync(
                OperationsEventNames.StockChanged,
                companyId.Value,
                request.ProductId,
                "Product",
                new { WarehouseId = request.WarehouseId, Quantity = request.Quantity, TransactionType = request.TransactionType, BalanceAfter = newBalance },
                cancellationToken);
        }

        var dto = await MapToDtoAsync(transaction.Id, companyId.Value, cancellationToken);
        return Result<StockTransactionDto>.Success(dto);
    }

    private async Task<StockTransactionDto> MapToDtoAsync(Guid transactionId, Guid companyId, CancellationToken cancellationToken)
    {
        var transaction = await _context.StockTransactions
            .Include(t => t.Product)
            .Include(t => t.Warehouse)
            .FirstOrDefaultAsync(t => t.Id == transactionId && t.CompanyId == companyId, cancellationToken);

        if (transaction == null)
        {
            throw new InvalidOperationException("Transaction not found after creation");
        }

        return new StockTransactionDto
        {
            Id = transaction.Id,
            ProductId = transaction.ProductId,
            ProductName = transaction.Product.Name,
            ProductSKU = transaction.Product.SKU,
            WarehouseId = transaction.WarehouseId,
            WarehouseName = transaction.Warehouse.Name,
            TransactionType = transaction.TransactionType,
            Quantity = transaction.Quantity,
            UnitCost = transaction.UnitCost,
            TotalCost = transaction.TotalCost,
            ReferenceNumber = transaction.ReferenceNumber,
            ReferenceType = transaction.ReferenceType,
            ReferenceId = transaction.ReferenceId,
            BatchNumber = transaction.BatchNumber,
            ExpiryDate = transaction.ExpiryDate,
            Notes = transaction.Notes,
            TransactionDate = transaction.TransactionDate,
            BalanceAfter = transaction.BalanceAfter,
            CreatedAt = transaction.CreatedAt
        };
    }
}

