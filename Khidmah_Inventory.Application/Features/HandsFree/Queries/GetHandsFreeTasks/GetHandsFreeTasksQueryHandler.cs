using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.HandsFree.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.HandsFree.Queries.GetHandsFreeTasks;

public class GetHandsFreeTasksQueryHandler : IRequestHandler<GetHandsFreeTasksQuery, Result<HandsFreeTasksResult>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetHandsFreeTasksQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<HandsFreeTasksResult>> Handle(GetHandsFreeTasksQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<HandsFreeTasksResult>.Failure("Company context is required.");

        var warehouse = await _context.Warehouses
            .AsNoTracking()
            .FirstOrDefaultAsync(w => w.Id == request.WarehouseId && w.CompanyId == companyId && w.IsDeleted == false, cancellationToken);
        if (warehouse == null)
            return Result<HandsFreeTasksResult>.Failure("Warehouse not found.");

        var sessionId = Guid.NewGuid();
        var session = new HandsFreeSessionDto
        {
            SessionId = sessionId,
            StartedAt = DateTime.UtcNow,
            WarehouseId = warehouse.Id,
            WarehouseName = warehouse.Name
        };

        var levels = await _context.StockLevels
            .AsNoTracking()
            .Where(sl => sl.CompanyId == companyId && sl.WarehouseId == request.WarehouseId && sl.IsDeleted == false && sl.Quantity > 0)
            .OrderBy(sl => sl.Product!.Name)
            .Take(request.MaxTasks)
            .Select(sl => new
            {
                sl.ProductId,
                sl.WarehouseId,
                ProductName = sl.Product != null ? sl.Product.Name : string.Empty,
                Barcode = sl.Product != null ? sl.Product.Barcode : null,
                SKU = sl.Product != null ? sl.Product.SKU : string.Empty,
                WarehouseCode = sl.Warehouse != null ? sl.Warehouse.Code : null,
                WarehouseName = sl.Warehouse != null ? sl.Warehouse.Name : null
            })
            .ToListAsync(cancellationToken);

        var tasks = new List<HandsFreeTaskDto>();
        int seq = 0;
        foreach (var sl in levels)
        {
            var loc = sl.WarehouseCode ?? sl.WarehouseName ?? "Main";
            tasks.Add(new HandsFreeTaskDto
            {
                TaskId = Guid.NewGuid(),
                ProductId = sl.ProductId,
                ProductName = sl.ProductName ?? "",
                Barcode = sl.Barcode,
                Sku = sl.SKU ?? "",
                Location = loc,
                WarehouseId = sl.WarehouseId,
                Quantity = 1,
                Sequence = ++seq
            });
        }

        return Result<HandsFreeTasksResult>.Success(new HandsFreeTasksResult
        {
            Session = session,
            Tasks = tasks,
            CurrentIndex = 0
        });
    }
}
