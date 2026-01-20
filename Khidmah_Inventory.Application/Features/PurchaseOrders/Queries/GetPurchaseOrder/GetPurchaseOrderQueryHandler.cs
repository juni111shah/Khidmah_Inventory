using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.PurchaseOrders.Models;

namespace Khidmah_Inventory.Application.Features.PurchaseOrders.Queries.GetPurchaseOrder;

public class GetPurchaseOrderQueryHandler : IRequestHandler<GetPurchaseOrderQuery, Result<PurchaseOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPurchaseOrderQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PurchaseOrderDto>> Handle(GetPurchaseOrderQuery request, CancellationToken cancellationToken)
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

        var dto = new PurchaseOrderDto
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

        return Result<PurchaseOrderDto>.Success(dto);
    }
}

