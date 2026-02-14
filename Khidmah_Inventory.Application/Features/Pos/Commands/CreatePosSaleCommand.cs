using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Constants;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Pos.Dtos;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Pos.Commands;

public class CreatePosSaleCommand : IRequest<Result<PosSaleDto>>
{
    public Guid PosSessionId { get; set; }
    public Guid CustomerId { get; set; }
    public string PaymentMethod { get; set; } = "Cash"; // Cash, Card
    public decimal AmountPaid { get; set; }
    public List<PosSaleItemDto> Items { get; set; } = new();
    public Guid WarehouseId { get; set; }
}

public class PosSaleItemDto
{
    public Guid ProductId { get; set; }
    public decimal Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal DiscountAmount { get; set; }
}

public class CreatePosSaleCommandHandler : IRequestHandler<CreatePosSaleCommand, Result<PosSaleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;
    private readonly IOperationsBroadcast? _broadcast;

    public CreatePosSaleCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser, IOperationsBroadcast? broadcast = null)
    {
        _context = context;
        _currentUser = currentUser;
        _broadcast = broadcast;
    }

    public async Task<Result<PosSaleDto>> Handle(CreatePosSaleCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue) return Result<PosSaleDto>.Failure("Company context required.");

        var session = await _context.PosSessions
            .FirstOrDefaultAsync(s => s.Id == request.PosSessionId && s.Status == "Open", cancellationToken);
        if (session == null) return Result<PosSaleDto>.Failure("No active POS session found.");

        var customer = await _context.Customers.FindAsync(request.CustomerId);
        if (customer == null) return Result<PosSaleDto>.Failure("Customer not found.");

        var orderNumber = await GeneratePosOrderNumberAsync(companyId.Value, cancellationToken);
        var salesOrder = new SalesOrder(companyId.Value, orderNumber, request.CustomerId, DateTime.UtcNow, _currentUser.UserId);

        decimal subTotal = 0;
        foreach (var item in request.Items)
        {
            var product = await _context.Products.FindAsync(item.ProductId);
            if (product == null) continue;

            var salesOrderItem = new SalesOrderItem(companyId.Value, salesOrder.Id, item.ProductId, item.Quantity, item.UnitPrice, _currentUser.UserId);
            salesOrderItem.Update(item.Quantity, item.UnitPrice, 0, 0, null, _currentUser.UserId);
            salesOrder.Items.Add(salesOrderItem);

            subTotal += (item.Quantity * item.UnitPrice) - item.DiscountAmount;

            // Reduce Stock
            var stockLevel = await _context.StockLevels
                .FirstOrDefaultAsync(sl => sl.ProductId == item.ProductId && sl.WarehouseId == request.WarehouseId, cancellationToken);

            if (stockLevel == null || stockLevel.Quantity < item.Quantity)
            {
                return Result<PosSaleDto>.Failure($"Insufficient stock for product {product.Name}");
            }

            var stockTransaction = new StockTransaction(companyId.Value, item.ProductId, request.WarehouseId, "StockOut", item.Quantity, product.CostPrice, _currentUser.UserId);
            stockTransaction.SetReference("POS", salesOrder.Id, orderNumber);
            stockTransaction.SetBalanceAfter(stockLevel.Quantity - item.Quantity);

            stockLevel.AdjustQuantity(-item.Quantity, product.CostPrice);
            _context.StockTransactions.Add(stockTransaction);
        }

        salesOrder.CalculateTotals();
        var changeAmount = request.AmountPaid - salesOrder.TotalAmount;
        salesOrder.SetPosData(session.Id, request.PaymentMethod, request.AmountPaid, changeAmount);
        salesOrder.UpdateStatus("Delivered", _currentUser.UserId); // POS sales are immediate

        _context.SalesOrders.Add(salesOrder);
        await _context.SaveChangesAsync(cancellationToken);

        if (_broadcast != null)
        {
            await _broadcast.BroadcastAsync(
                OperationsEventNames.SaleCompleted,
                companyId.Value,
                salesOrder.Id,
                "SalesOrder",
                new { OrderNumber = salesOrder.OrderNumber, TotalAmount = salesOrder.TotalAmount, IsPos = true },
                cancellationToken);
            await _broadcast.BroadcastAsync(
                OperationsEventNames.StockChanged,
                companyId.Value,
                null,
                "Warehouse",
                new { WarehouseId = request.WarehouseId, Message = "POS sale completed" },
                cancellationToken);
        }

        return Result<PosSaleDto>.Success(new PosSaleDto
        {
            Id = salesOrder.Id,
            OrderNumber = salesOrder.OrderNumber,
            CustomerId = salesOrder.CustomerId,
            CustomerName = customer.Name,
            OrderDate = salesOrder.OrderDate,
            TotalAmount = salesOrder.TotalAmount,
            PaymentMethod = salesOrder.PaymentMethod!,
            AmountPaid = salesOrder.AmountPaid,
            ChangeAmount = salesOrder.ChangeAmount
        });
    }

    private async Task<string> GeneratePosOrderNumberAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var prefix = $"POS-{today:yyyyMMdd}-";
        var count = await _context.SalesOrders.CountAsync(so => so.CompanyId == companyId && so.IsPos && so.OrderNumber.StartsWith(prefix), cancellationToken);
        return $"{prefix}{(count + 1):D4}";
    }
}
