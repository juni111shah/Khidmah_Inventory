using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.SalesOrders.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.SalesOrders.Commands.CreateSalesOrder;

public class CreateSalesOrderCommandHandler : IRequestHandler<CreateSalesOrderCommand, Result<SalesOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateSalesOrderCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<SalesOrderDto>> Handle(CreateSalesOrderCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<SalesOrderDto>.Failure("Company context is required");

        var customer = await _context.Customers
            .FirstOrDefaultAsync(c => c.Id == request.CustomerId && c.CompanyId == companyId.Value && !c.IsDeleted, cancellationToken);

        if (customer == null)
            return Result<SalesOrderDto>.Failure("Customer not found.");

        var orderNumber = await GenerateOrderNumberAsync(companyId.Value, cancellationToken);
        var salesOrder = new SalesOrder(companyId.Value, orderNumber, request.CustomerId, request.OrderDate, _currentUser.UserId);
        salesOrder.Update(request.CustomerId, request.OrderDate, request.ExpectedDeliveryDate, request.Notes, request.TermsAndConditions, _currentUser.UserId);

        foreach (var itemDto in request.Items)
        {
            var product = await _context.Products
                .FirstOrDefaultAsync(p => p.Id == itemDto.ProductId && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

            if (product == null)
                return Result<SalesOrderDto>.Failure($"Product with ID {itemDto.ProductId} not found.");

            var orderItem = new SalesOrderItem(companyId.Value, salesOrder.Id, itemDto.ProductId, itemDto.Quantity, itemDto.UnitPrice, _currentUser.UserId);
            orderItem.Update(itemDto.Quantity, itemDto.UnitPrice, itemDto.DiscountPercent, itemDto.TaxPercent, itemDto.Notes, _currentUser.UserId);
            salesOrder.Items.Add(orderItem);
        }

        salesOrder.CalculateTotals();
        _context.SalesOrders.Add(salesOrder);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(salesOrder.Id, companyId.Value, cancellationToken);
        return Result<SalesOrderDto>.Success(dto);
    }

    private async Task<string> GenerateOrderNumberAsync(Guid companyId, CancellationToken cancellationToken)
    {
        var today = DateTime.UtcNow;
        var prefix = $"SO-{today:yyyyMM}-";
        var lastOrder = await _context.SalesOrders
            .Where(so => so.CompanyId == companyId && so.OrderNumber.StartsWith(prefix))
            .OrderByDescending(so => so.OrderNumber)
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

    private async Task<SalesOrderDto> MapToDtoAsync(Guid salesOrderId, Guid companyId, CancellationToken cancellationToken)
    {
        var salesOrder = await _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
            .FirstOrDefaultAsync(so => so.Id == salesOrderId && so.CompanyId == companyId, cancellationToken);

        if (salesOrder == null)
            throw new InvalidOperationException("Sales order not found after creation");

        return new SalesOrderDto
        {
            Id = salesOrder.Id,
            OrderNumber = salesOrder.OrderNumber,
            CustomerId = salesOrder.CustomerId,
            CustomerName = salesOrder.Customer.Name,
            OrderDate = salesOrder.OrderDate,
            ExpectedDeliveryDate = salesOrder.ExpectedDeliveryDate,
            Status = salesOrder.Status,
            SubTotal = salesOrder.SubTotal,
            TaxAmount = salesOrder.TaxAmount,
            DiscountAmount = salesOrder.DiscountAmount,
            TotalAmount = salesOrder.TotalAmount,
            Notes = salesOrder.Notes,
            TermsAndConditions = salesOrder.TermsAndConditions,
            Items = salesOrder.Items.Select(item => new SalesOrderItemDto
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
                DeliveredQuantity = item.DeliveredQuantity,
                Notes = item.Notes
            }).ToList(),
            CreatedAt = salesOrder.CreatedAt,
            UpdatedAt = salesOrder.UpdatedAt
        };
    }
}

