using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Reports.Models;

namespace Khidmah_Inventory.Application.Features.Reports.Queries.GetPurchaseReport;

public class GetPurchaseReportQueryHandler : IRequestHandler<GetPurchaseReportQuery, Result<PurchaseReportDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetPurchaseReportQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PurchaseReportDto>> Handle(GetPurchaseReportQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PurchaseReportDto>.Failure("Company context is required");

        var query = _context.PurchaseOrders
            .Include(po => po.Supplier)
            .Where(po => po.CompanyId == companyId.Value && 
                !po.IsDeleted &&
                po.OrderDate >= request.FromDate &&
                po.OrderDate <= request.ToDate);

        if (request.SupplierId.HasValue)
            query = query.Where(po => po.SupplierId == request.SupplierId.Value);

        var orders = await query.OrderByDescending(po => po.OrderDate).ToListAsync(cancellationToken);

        var report = new PurchaseReportDto
        {
            FromDate = request.FromDate,
            ToDate = request.ToDate,
            TotalPurchases = orders.Sum(o => o.TotalAmount),
            TotalOrders = orders.Count,
            Items = orders.Select(po => new PurchaseReportItemDto
            {
                Date = po.OrderDate,
                OrderNumber = po.OrderNumber,
                SupplierName = po.Supplier.Name,
                Amount = po.TotalAmount,
                Status = po.Status
            }).ToList()
        };

        return Result<PurchaseReportDto>.Success(report);
    }
}

