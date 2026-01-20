using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.PurchaseOrders.Commands.CreatePurchaseOrder;

public class CreatePurchaseOrderCommandHandler : IRequestHandler<CreatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreatePurchaseOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PurchaseOrderDto>> Handle(CreatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PurchaseOrderDto>.Failure("Company context is required");

        // Validate supplier
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.SupplierId && s.CompanyId == companyId.Value && !s.IsDeleted, cancellationToken);

        if (supplier == null)
            return Result<PurchaseOrderDto>.Failure("Supplier not found.");

        // Generate order number
        var orderNumber = await GenerateOrderNumberAsync(companyId.Value, cancellationToken);

        // Create purchase order
        var purchaseOrder = new PurchaseOrder(companyId.Value, orderNumber, request.SupplierId, request.OrderDate, _currentUser.UserId);
        purchaseOrder.Update(request.SupplierId, request.OrderDate, request.ExpectedDeliveryDate, request.Notes, request.TermsAndConditions, _currentUser.UserId);

        // Validate and add items
        foreach (var itemDto in request.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == itemDto.ProductId && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

            if (product == null)
                return Result<PurchaseOrderDto>.Failure($"Product with ID {itemDto.ProductId} not found.");

            var orderItem = new PurchaseOrderItem(
                companyId.Value,
                purchaseOrder.Id,
                itemDto.ProductId,
                itemDto.Quantity,
                itemDto.UnitPrice,
                _currentUser.UserId);

            orderItem.Update(itemDto.Quantity, itemDto.UnitPrice, itemDto.DiscountPercent, itemDto.TaxPercent, itemDto.Notes, _currentUser.UserId);
            purchaseOrder.Items.Add(orderItem);
        }

        purchaseOrder.CalculateTotals();
        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(purchaseOrder.Id, companyId.Value, cancellationToken);
        return Result<PurchaseOrderDto>.Success(dto);
    }

    private async Task<string> GenerateOrderNumberAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var prefix = $"PO-{today:yyyyMM}-";
        var lastOrder = await _context.PurchaseOrders
            .Where(po => po.CompanyId == companyId && po.OrderNumber.StartsWith(prefix))
            .OrderByDescending(po => po.OrderNumber)
            .FirstOrDefaultAsync(cancellationToken);

        int sequence = 1;
        if (lastOrder != null)
        {
            var lastSequence = lastOrder.OrderNumber.Replace(prefix, "");
            if (int.TryParse(lastSequence, out var lastNum))
                sequence = lastNum + 1;
        }

        return $"{prefix}{sequence:D4}";
    }

    private async Task<PurchaseOrderDto> MapToDtoAsync(Guid purchaseOrderId, Guid companyId, CancellationToken cancellationToken)
    {
        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(po => po.Id == purchaseOrderId && po.CompanyId == companyId, cancellationToken);

        if (purchaseOrder == null)
            throw new InvalidOperationException("Purchase order not found after creation");

        return new PurchaseOrderDto
        {
            Id = purchaseOrder.Id,
            OrderNumber = purchaseOrder.OrderNumber,
            SupplierId = purchaseOrder.SupplierId,
            SupplierName = purchaseOrder.Supplier.Name,
            OrderDate = purchaseOrder.OrderDate,
            ExpectedDeliveryDate = purchaseOrder.ExpectedDeliveryDate,
            Status = purchaseOrder.Status,
            SubTotal = purchaseOrder.SubTotal,
            TaxAmount = purchaseOrder.TaxAmount,
            DiscountAmount = purchaseOrder.DiscountAmount,
            TotalAmount = purchaseOrder.TotalAmount,
            Notes = purchaseOrder.Notes,
            TermsAndConditions = purchaseOrder.TermsAndConditions,
            Items = purchaseOrder.Items.Select(item => new PurchaseOrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                ProductSKU = item.Product.SKU,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountPercent = item.DiscountPercent,
                DiscountAmount = item.DiscountAmount,
                TaxPercent = item.TaxPercent,
                TaxAmount = item.TaxAmount,
                LineTotal = item.LineTotal,
                ReceivedQuantity = item.ReceivedQuantity,
                Notes = item.Notes
            }).ToList(),
            CreatedAt = purchaseOrder.CreatedAt,
            UpdatedAt = purchaseOrder.UpdatedAt
        };
    }
}

