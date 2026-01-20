using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Reordering.Commands.GeneratePurchaseOrderFromSuggestions;

public class GeneratePurchaseOrderFromSuggestionsCommandHandler : IRequestHandler<GeneratePurchaseOrderFromSuggestionsCommand, Result<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GeneratePurchaseOrderFromSuggestionsCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PurchaseOrderDto>> Handle(GeneratePurchaseOrderFromSuggestionsCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PurchaseOrderDto>.Failure("Company context is required");

        // Validate supplier
        var supplier = await _context.Suppliers
            .FirstOrDefaultAsync(s => s.Id == request.SupplierId && s.CompanyId == companyId.Value && !s.IsDeleted, cancellationToken);

        if (supplier == null)
            return Result<PurchaseOrderDto>.Failure("Supplier not found.");

        // Generate PO number
        var today = DateTime.UtcNow;
        var poPrefix = $"PO-{today:yyyyMM}-";
        var lastPO = await _context.PurchaseOrders
            .Where(po => po.CompanyId == companyId.Value && po.OrderNumber.StartsWith(poPrefix))
            .OrderByDescending(po => po.OrderNumber)
            .FirstOrDefaultAsync(cancellationToken);

        int sequenceNumber = 1;
        if (lastPO != null)
        {
            var lastSequence = lastPO.OrderNumber.Replace(poPrefix, "");
            if (int.TryParse(lastSequence, out int lastNum))
                sequenceNumber = lastNum + 1;
        }

        var orderNumber = $"{poPrefix}{sequenceNumber:D4}";

        // Create purchase order
        var purchaseOrder = new PurchaseOrder(
            companyId.Value,
            orderNumber,
            request.SupplierId,
            DateTime.UtcNow,
            _currentUser.UserId);

        if (request.ExpectedDeliveryDate.HasValue)
            purchaseOrder.SetExpectedDeliveryDate(request.ExpectedDeliveryDate.Value);

        if (!string.IsNullOrWhiteSpace(request.Notes))
            purchaseOrder.Update(supplier.Id, DateTime.UtcNow, request.ExpectedDeliveryDate, request.Notes, null, _currentUser.UserId);

        // Add items
        foreach (var item in request.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == item.ProductId && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

            if (product == null)
                continue;

            var unitPrice = item.UnitPrice ?? product.PurchasePrice;
            var purchaseOrderItem = new PurchaseOrderItem(
                companyId.Value,
                purchaseOrder.Id,
                item.ProductId,
                item.Quantity,
                unitPrice,
                _currentUser.UserId);

            purchaseOrder.AddItem(purchaseOrderItem);
        }

        purchaseOrder.CalculateTotals();
        purchaseOrder.SetStatus("Draft");

        _context.PurchaseOrders.Add(purchaseOrder);
        await _context.SaveChangesAsync(cancellationToken);

        // Load with navigation properties
        var poWithNav = await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(po => po.Id == purchaseOrder.Id && po.CompanyId == companyId.Value, cancellationToken);

        var dto = new PurchaseOrderDto
        {
            Id = poWithNav!.Id,
            OrderNumber = poWithNav.OrderNumber,
            SupplierId = poWithNav.SupplierId,
            SupplierName = poWithNav.Supplier.Name,
            OrderDate = poWithNav.OrderDate,
            ExpectedDeliveryDate = poWithNav.ExpectedDeliveryDate,
            Status = poWithNav.Status,
            SubTotal = poWithNav.SubTotal,
            TaxAmount = poWithNav.TaxAmount,
            DiscountAmount = poWithNav.DiscountAmount,
            TotalAmount = poWithNav.TotalAmount,
            Notes = poWithNav.Notes,
            Items = poWithNav.Items.Select(item => new PurchaseOrderItemDto
            {
                Id = item.Id,
                ProductId = item.ProductId,
                ProductName = item.Product.Name,
                ProductSKU = item.Product.SKU,
                Quantity = item.Quantity,
                UnitPrice = item.UnitPrice,
                DiscountAmount = item.DiscountAmount,
                TaxAmount = item.TaxAmount,
                LineTotal = item.LineTotal
            }).ToList(),
            CreatedAt = poWithNav.CreatedAt
        };

        return Result<PurchaseOrderDto>.Success(dto);
    }
}

