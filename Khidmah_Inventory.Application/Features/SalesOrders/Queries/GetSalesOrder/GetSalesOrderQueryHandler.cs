using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.SalesOrders.Models;

namespace Khidmah_Inventory.Application.Features.SalesOrders.Queries.GetSalesOrder;

public class GetSalesOrderQueryHandler : IRequestHandler<GetSalesOrderQuery, Result<SalesOrderDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSalesOrderQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<SalesOrderDto>> Handle(GetSalesOrderQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<SalesOrderDto>.Failure("Company context is required");

        var salesOrder = await _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
            .Where(so => so.Id == request.Id && so.CompanyId == companyId.Value && !so.IsDeleted)
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

        if (salesOrder == null)
            return Result<SalesOrderDto>.Failure("Sales order not found");

        return Result<SalesOrderDto>.Success(salesOrder);
    }
}