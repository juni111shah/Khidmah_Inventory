using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Commands.CreateStockTransaction;

namespace Khidmah_Inventory.Application.Features.HandsFree.Commands.CompleteHandsFreeTask;

public class CompleteHandsFreeTaskCommandHandler : IRequestHandler<CompleteHandsFreeTaskCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IMediator _mediator;

    public CompleteHandsFreeTaskCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser,
        IMediator mediator)
    {
        _context = context;
        _currentUser = currentUser;
        _mediator = mediator;
    }

    public async Task<Result> Handle(CompleteHandsFreeTaskCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required.");

        var product = await _context.Products
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.CompanyId == companyId && p.IsDeleted == false, cancellationToken);
        if (product == null)
            return Result.Failure("Product not found.");

        var expectedBarcode = (product.Barcode ?? product.SKU ?? "").Trim();
        var scanned = (request.BarcodeScanned ?? "").Trim();
        if (!string.IsNullOrEmpty(expectedBarcode) && !string.Equals(expectedBarcode, scanned, StringComparison.OrdinalIgnoreCase))
            return Result.Failure("Wrong barcode. Expected " + expectedBarcode + ".");

        var stockLevel = await _context.StockLevels
            .FirstOrDefaultAsync(sl => sl.ProductId == request.ProductId && sl.WarehouseId == request.WarehouseId && sl.CompanyId == companyId && sl.IsDeleted == false, cancellationToken);
        if (stockLevel == null)
            return Result.Failure("No stock for this product in this warehouse.");
        if (stockLevel.Quantity < request.QuantityPicked)
            return Result.Failure("Insufficient quantity. Available: " + stockLevel.Quantity + ".");

        var txResult = await _mediator.Send(new CreateStockTransactionCommand
        {
            ProductId = request.ProductId,
            WarehouseId = request.WarehouseId,
            TransactionType = "StockOut",
            Quantity = request.QuantityPicked,
            Notes = "Hands-free pick",
            ReferenceType = "HandsFree",
            ReferenceId = request.TaskId
        }, cancellationToken);

        return txResult.Succeeded ? Result.Success() : Result.Failure(txResult.Errors);
    }
}
