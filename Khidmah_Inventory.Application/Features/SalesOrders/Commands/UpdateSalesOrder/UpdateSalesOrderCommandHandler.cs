using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.SalesOrders.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.SalesOrders.Commands.UpdateSalesOrder;

public class UpdateSalesOrderCommandHandler : IRequestHandler<UpdateSalesOrderCommand, Result<SalesOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateSalesOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<SalesOrderDto>> Handle(UpdateSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<SalesOrderDto>.Failure("Company context is required");

        var salesOrder = await _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(so => so.Id == request.Id && so.CompanyId == companyId.Value && !so.IsDeleted, cancellationToken);

        if (salesOrder == null)
            return Result<SalesOrderDto>.Failure("Sales order not found.");

        if (salesOrder.Status == "Approved" || salesOrder.Status == "Posted")
            return Result<SalesOrderDto>.Failure("Approved or posted orders are locked and cannot be edited.");

        var productIds = request.Items.Select(i => i.ProductId).Distinct().ToList();
        var validProducts = await _context.Products
            .Where(p => productIds.Contains(p.Id) && p.CompanyId == companyId.Value && !p.IsDeleted)
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        var missingProducts = productIds.Except(validProducts).ToList();
        if (missingProducts.Count > 0)
            return Result<SalesOrderDto>.Failure($"Some products were not found: {string.Join(", ", missingProducts)}");

        salesOrder.Update(
            request.CustomerId,
            request.OrderDate,
            request.ExpectedDeliveryDate,
            request.Notes,
            request.TermsAndConditions,
            _currentUser.UserId);

        foreach (var existingItem in salesOrder.Items.Where(i => !i.IsDeleted))
        {
            existingItem.MarkAsDeleted(_currentUser.UserId);
        }

        foreach (var itemDto in request.Items)
        {
            var orderItem = new SalesOrderItem(
                companyId.Value,
                salesOrder.Id,
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

            salesOrder.Items.Add(orderItem);
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            salesOrder.UpdateStatus(request.Status.Trim(), _currentUser.UserId);
        }

        salesOrder.CalculateTotals();
        await _context.SaveChangesAsync(cancellationToken);

        var updated = await _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
            .Where(so => so.Id == request.Id && so.CompanyId == companyId.Value)
            .Select(so => new SalesOrderDto
            {
                Id = so.Id,
                OrderNumber = so.OrderNumber,
                CustomerId = so.CustomerId,
                CustomerName = so.Customer != null ? so.Customer.Name : "Unknown Customer",
                OrderDate = so.OrderDate,
                ExpectedDeliveryDate = so.ExpectedDeliveryDate,
                Status = so.Status,
                SubTotal = so.SubTotal,
                TaxAmount = so.TaxAmount,
                DiscountAmount = so.DiscountAmount,
                TotalAmount = so.TotalAmount,
                Notes = so.Notes,
                TermsAndConditions = so.TermsAndConditions,
                CreatedAt = so.CreatedAt,
                UpdatedAt = so.UpdatedAt,
                Items = so.Items.Where(item => !item.IsDeleted).Select(item => new SalesOrderItemDto
                {
                    Id = item.Id,
                    ProductId = item.ProductId,
                    ProductName = item.Product != null ? item.Product.Name : "Unknown Product",
                    ProductSKU = item.Product != null ? item.Product.SKU : "N/A",
                    Quantity = item.Quantity,
                    UnitPrice = item.UnitPrice,
                    DiscountPercent = item.DiscountPercent,
                    DiscountAmount = item.DiscountAmount,
                    TaxPercent = item.TaxPercent,
                    TaxAmount = item.TaxAmount,
                    LineTotal = item.LineTotal,
                    DeliveredQuantity = item.DeliveredQuantity,
                    Notes = item.Notes
                }).ToList()
            })
            .FirstOrDefaultAsync(cancellationToken);

        if (updated == null)
            return Result<SalesOrderDto>.Failure("Sales order could not be retrieved after update.");

        return Result<SalesOrderDto>.Success(updated);
    }
}
