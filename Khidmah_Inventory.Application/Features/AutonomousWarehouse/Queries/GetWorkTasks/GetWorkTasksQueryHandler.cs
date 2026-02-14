using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;
using Khidmah_Inventory.Domain.Enums;

namespace Khidmah_Inventory.Application.Features.AutonomousWarehouse.Queries.GetWorkTasks;

public class GetWorkTasksQueryHandler : IRequestHandler<GetWorkTasksQuery, Result<List<WorkTaskDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWorkTasksQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<WorkTaskDto>>> Handle(GetWorkTasksQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<WorkTaskDto>>.Failure("Company context is required.");

        var query = _context.WorkTasks
            .Include(t => t.Warehouse)
            .Include(t => t.Product)
            .Where(t => t.CompanyId == companyId.Value && t.WarehouseId == request.WarehouseId && !t.IsDeleted);

        if (request.AssignedToId.HasValue)
            query = query.Where(t => t.AssignedToId == request.AssignedToId.Value);
        if (request.Status.HasValue)
            query = query.Where(t => (int)t.Status == request.Status.Value);
        if (request.Type.HasValue)
            query = query.Where(t => (int)t.Type == request.Type.Value);

        var tasks = await query
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .Take(request.MaxCount)
            .ToListAsync(cancellationToken);

        var list = tasks.Select(t => new WorkTaskDto
        {
            Id = t.Id,
            WarehouseId = t.WarehouseId,
            WarehouseName = t.Warehouse?.Name ?? "",
            Type = t.Type,
            TypeDisplay = t.Type.ToString(),
            Priority = t.Priority,
            Status = t.Status,
            StatusDisplay = t.Status.ToString(),
            AssignedToId = t.AssignedToId,
            AssignedToType = t.AssignedToType,
            MapBinId = t.MapBinId,
            LocationCode = t.LocationCode,
            ProductId = t.ProductId,
            ProductName = t.ProductName,
            ProductSku = t.ProductSku,
            Quantity = t.Quantity,
            SourceOrderId = t.SourceOrderId,
            AssignedAt = t.AssignedAt,
            StartedAt = t.StartedAt,
            CompletedAt = t.CompletedAt,
            CreatedAt = t.CreatedAt
        }).ToList();

        return Result<List<WorkTaskDto>>.Success(list);
    }
}
