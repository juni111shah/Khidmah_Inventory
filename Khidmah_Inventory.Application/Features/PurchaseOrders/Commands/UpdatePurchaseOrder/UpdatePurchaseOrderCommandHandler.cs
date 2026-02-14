using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.PurchaseOrders.Commands.UpdatePurchaseOrder;

public class UpdatePurchaseOrderCommandHandler : IRequestHandler<UpdatePurchaseOrderCommand, Result<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdatePurchaseOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PurchaseOrderDto>> Handle(UpdatePurchaseOrderCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PurchaseOrderDto>.Failure("Company context is required");

        var purchaseOrder = await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(po => po.Id == request.Id && po.CompanyId == companyId.Value && !po.IsDeleted, cancellationToken);

        if (purchaseOrder == null)
            return Result<PurchaseOrderDto>.Failure("Purchase order not found.");

        if (purchaseOrder.Status == "Approved" || purchaseOrder.Status == "Posted")
            return Result<PurchaseOrderDto>.Failure("Approved or posted orders are locked and cannot be edited.");

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var validProducts = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.CompanyId == companyId.Value && !p.IsDeleted)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        var missingProducts = productIds.Except(validProducts).ToList();
        if (missingProducts.Count > 0)
            return Result<PurchaseOrderDto>.Failure($"Some products were not found: {string.Join(", ", missingProducts)}");

        purchaseOrder.Update(
            request.SupplierId,
            request.OrderDate,
            request.ExpectedDeliveryDate,
            request.Notes,
            request.TermsAndConditions,
            _currentUser.UserId);

        foreach (var existingItem in purchaseOrder.Items.Where(i => !i.IsDeleted))
        {
            existingItem.MarkAsDeleted(_currentUser.UserId);
        }

        foreach (var itemDto in request.Items)
        {
            var orderItem = new PurchaseOrderItem(
                companyId.Value,
                purchaseOrder.Id,
                itemDto.ProductId,
                itemDto.Quantity,
                itemDto.UnitPrice,
                _currentUser.UserId);

            orderItem.Update(
                itemDto.Quantity,
                itemDto.UnitPrice,
                itemDto.DiscountPercent,
                itemDto.TaxPercent,
                itemDto.Notes,
                _currentUser.UserId);

            purchaseOrder.Items.Add(orderItem);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            purchaseOrder.UpdateStatus(request.Status.Trim(), _currentUser.UserId);
        }

        purchaseOrder.CalculateTotals();
        await _context.SaveChangesAsync(cancellationToken);

        var updated = await _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Include(po => po.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(po => po.Id == request.Id && po.CompanyId == companyId.Value, cancellationToken);

        if (updated == null)
            return Result<PurchaseOrderDto>.Failure("Purchase order could not be retrieved after update.");

        var dto = new PurchaseOrderDto
        {
            Id = updated.Id,
            OrderNumber = updated.OrderNumber,
            SupplierId = updated.SupplierId,
            SupplierName = updated.Supplier.Name,
            OrderDate = updated.OrderDate,
            ExpectedDeliveryDate = updated.ExpectedDeliveryDate,
            Status = updated.Status,
            SubTotal = updated.SubTotal,
            TaxAmount = updated.TaxAmount,
            DiscountAmount = updated.DiscountAmount,
            TotalAmount = updated.TotalAmount,
            Notes = updated.Notes,
            TermsAndConditions = updated.TermsAndConditions,
            Items = updated.Items.Select(item => new PurchaseOrderItemDto
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
            CreatedAt = updated.CreatedAt,
            UpdatedAt = updated.UpdatedAt
        };

        return Result<PurchaseOrderDto>.Success(dto);
    }
}
