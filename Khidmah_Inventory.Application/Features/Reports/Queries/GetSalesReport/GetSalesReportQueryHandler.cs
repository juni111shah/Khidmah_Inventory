using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetSalesReport;

public class GetSalesReportQueryHandler : IRequestHandler<GetSalesReportQuery, Result<SalesReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetSalesReportQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<SalesReportDto>> Handle(GetSalesReportQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<SalesReportDto>.Failure("Company context is required");

        var query = _context.SalesOrders
            .Include(so => so.Customer)
            .Include(so => so.Items)
                .ThenInclude(item => item.Product)
            .Where(so => so.CompanyId == companyId.Value && 
                !so.IsDeleted &&
                so.OrderDate >= request.FromDate &&
                so.OrderDate <= request.ToDate);

        if (request.CustomerId.HasValue)
            query = query.Where(so => so.CustomerId == request.CustomerId.Value);

        var orders = await query.ToListAsync(cancellationToken);

        var report = new SalesReportDto
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            TotalSales = orders.Sum(o => o.TotalAmount),
            TotalOrders = orders.Count
        };

        // Calculate cost and profit
        decimal totalCost = 0;
        foreach (var order in orders)
        {
            foreach (var item in order.Items)
            {
                var stockLevel = await _context.StockLevels
                    .FirstOrDefaultAsync(sl => sl.ProductId == item.ProductId && 
                        sl.CompanyId == companyId.Value, cancellationToken);

                var cost = stockLevel?.AverageCost ?? item.Product.PurchasePrice;
                totalCost += item.Quantity * cost;
            }
        }

        report.TotalCost = totalCost;
        report.TotalProfit = report.TotalSales - report.TotalCost;
        report.ProfitMargin = report.TotalSales > 0 
            ? (report.TotalProfit / report.TotalSales) * 100 
            : 0;

        // Report items
        report.Items = orders.Select(order =>
        {
            decimal orderCost = 0;
            foreach (var item in order.Items)
            {
                var stockLevel = _context.StockLevels
                    .FirstOrDefault(sl => sl.ProductId == item.ProductId && 
                        sl.CompanyId == companyId.Value);

                var cost = stockLevel?.AverageCost ?? item.Product.PurchasePrice;
                orderCost += item.Quantity * cost;
            }

            return new SalesReportItemDto
            {
                Date = order.OrderDate,
                OrderNumber = order.OrderNumber,
                CustomerName = order.Customer.Name,
                Amount = order.TotalAmount,
                Cost = orderCost,
                Profit = order.TotalAmount - orderCost
            };
        }).ToList();

        return Result<SalesReportDto>.Success(report);
    }
}

