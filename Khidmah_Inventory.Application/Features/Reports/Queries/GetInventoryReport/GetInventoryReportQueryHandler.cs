using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetInventoryReport;

public class GetInventoryReportQueryHandler : IRequestHandler<GetInventoryReportQuery, Result<InventoryReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetInventoryReportQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<InventoryReportDto>> Handle(GetInventoryReportQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<InventoryReportDto>.Failure("Company context is required");

        var query = _context.StockLevels
            .Include(sl => sl.Product)
                .ThenInclude(p => p.Category)
            .Include(sl => sl.Warehouse)
            .Where(sl => sl.CompanyId == companyId.Value);

        if (request.WarehouseId.HasValue)
            query = query.Where(sl => sl.WarehouseId == request.WarehouseId.Value);

        if (request.CategoryId.HasValue)
            query = query.Where(sl => sl.Product.CategoryId == request.CategoryId.Value);

        if (request.LowStockOnly == true)
            query = query.Where(sl => sl.Product.MinStockLevel.HasValue && 
                sl.Quantity <= sl.Product.MinStockLevel.Value);

        var stockLevels = await query.ToListAsync(cancellationToken);

        var report = new InventoryReportDto
        {
            TotalStockValue = stockLevels.Sum(sl => sl.Quantity * (sl.AverageCost ?? sl.Product.PurchasePrice)),
            TotalProducts = stockLevels.Select(sl => sl.ProductId).Distinct().Count(),
            LowStockItems = stockLevels.Count(sl => sl.Product.MinStockLevel.HasValue && 
                sl.Quantity <= sl.Product.MinStockLevel.Value && sl.Quantity > 0),
            OutOfStockItems = stockLevels.Count(sl => sl.Quantity == 0)
        };

        report.Items = stockLevels.Select(sl =>
        {
            var stockValue = sl.Quantity * (sl.AverageCost ?? sl.Product.PurchasePrice);
            string status = "Normal";
            if (sl.Quantity == 0)
                status = "Out";
            else if (sl.Product.MinStockLevel.HasValue && sl.Quantity <= sl.Product.MinStockLevel.Value)
                status = "Low";

            return new InventoryReportItemDto
            {
                ProductId = sl.ProductId,
                ProductName = sl.Product.Name,
                ProductSKU = sl.Product.SKU,
                CategoryName = sl.Product.Category?.Name ?? "Uncategorized",
                WarehouseName = sl.Warehouse.Name,
                Quantity = sl.Quantity,
                AverageCost = sl.AverageCost ?? sl.Product.PurchasePrice,
                StockValue = stockValue,
                MinStockLevel = sl.Product.MinStockLevel,
                MaxStockLevel = sl.Product.MaxStockLevel,
                Status = status
            };
        }).OrderByDescending(i => i.StockValue).ToList();

        return Result<InventoryReportDto>.Success(report);
    }
}

