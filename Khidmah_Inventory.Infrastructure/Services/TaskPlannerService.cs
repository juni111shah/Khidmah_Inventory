using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;
using Khidmah_Inventory.Domain.Entities;
using Khidmah_Inventory.Domain.Enums;

namespace Khidmah_Inventory.Infrastructure.Services;

public class TaskPlannerService : ITaskPlanner
{
    private readonly IApplicationDbContext _context;

    public TaskPlannerService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<TaskPlanResult> PlanFromOrdersAsync(Guid companyId, Guid warehouseId, OrderTaskRequest request, CancellationToken cancellationToken = default)
    {
        var created = new List<WorkTask>();
        if (request.SalesOrderIds != null && request.SalesOrderIds.Any())
        {
            var orders = await _context.SalesOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.CompanyId == companyId && request.SalesOrderIds.Contains(o.Id) && !o.IsDeleted)
                .ToListAsync(cancellationToken);
            foreach (var order in orders)
            {
                foreach (var item in order.Items.Where(i => !i.IsDeleted))
                {
                    var qty = item.Quantity - (item.DeliveredQuantity ?? 0);
                    if (qty <= 0) continue;
                    var task = new WorkTask(
                        companyId,
                        warehouseId,
                        WorkTaskType.Pick,
                        priority: 10,
                        locationCode: null,
                        mapBinId: null,
                        item.ProductId,
                        item.Product?.Name,
                        item.Product?.SKU,
                        qty,
                        order.Id,
                        item.Id,
                        null);
                    _context.WorkTasks.Add(task);
                    created.Add(task);
                }
            }
        }

        if (request.PurchaseOrderIds != null && request.PurchaseOrderIds.Any())
        {
            var orders = await _context.PurchaseOrders
                .Include(o => o.Items)
                .ThenInclude(i => i.Product)
                .Where(o => o.CompanyId == companyId && request.PurchaseOrderIds.Contains(o.Id) && !o.IsDeleted)
                .ToListAsync(cancellationToken);
            foreach (var order in orders)
            {
                foreach (var item in order.Items.Where(i => !i.IsDeleted))
                {
                    var task = new WorkTask(
                        companyId,
                        warehouseId,
                        WorkTaskType.Putaway,
                        priority: 5,
                        locationCode: null,
                        mapBinId: null,
                        item.ProductId,
                        item.Product?.Name,
                        item.Product?.SKU,
                        item.Quantity,
                        order.Id,
                        item.Id,
                        null);
                    _context.WorkTasks.Add(task);
                    created.Add(task);
                }
            }
        }

        await _context.SaveChangesAsync(cancellationToken);
        var warehouse = await _context.Warehouses.FirstOrDefaultAsync(w => w.Id == warehouseId, cancellationToken);
        var dtos = created.Select(t => MapToDto(t, warehouse?.Name)).ToList();
        return new TaskPlanResult { CreatedTasks = dtos, TotalCount = dtos.Count };
    }

    public async Task<List<WorkTaskDto>> PrioritizeAsync(Guid companyId, IEnumerable<Guid> taskIds, CancellationToken cancellationToken = default)
    {
        var ids = taskIds.ToList();
        if (ids.Count == 0) return new List<WorkTaskDto>();

        var tasks = await _context.WorkTasks
            .Include(t => t.Warehouse)
            .Include(t => t.Product)
            .Where(t => t.CompanyId == companyId && ids.Contains(t.Id) && !t.IsDeleted)
            .OrderByDescending(t => t.Priority)
            .ThenBy(t => t.CreatedAt)
            .ToListAsync(cancellationToken);
        return tasks.Select(t => MapToDto(t, t.Warehouse?.Name)).ToList();
    }

    public async Task<AssignResult> AssignToAgentsAsync(Guid companyId, Guid warehouseId, IReadOnlyList<Guid> taskIds, CancellationToken cancellationToken = default)
    {
        if (taskIds.Count == 0)
            return new AssignResult();

        var tasks = await _context.WorkTasks
            .Where(t => t.CompanyId == companyId && t.WarehouseId == warehouseId && taskIds.Contains(t.Id) && !t.IsDeleted && t.Status == WorkTaskStatus.Pending)
            .ToListAsync(cancellationToken);

        var assignedCountByUser = await _context.WorkTasks
            .Where(t => t.CompanyId == companyId && t.WarehouseId == warehouseId && !t.IsDeleted && t.Status != WorkTaskStatus.Completed && t.Status != WorkTaskStatus.Cancelled && t.AssignedToId != null)
            .GroupBy(t => t.AssignedToId)
            .Select(g => new { UserId = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var userIds = await _context.UserCompanies
            .Where(uc => uc.CompanyId == companyId)
            .Select(uc => uc.UserId)
            .Distinct()
            .ToListAsync(cancellationToken);

        var orderedUserIds = userIds
            .OrderBy(uid => assignedCountByUser.FirstOrDefault(c => c.UserId == uid)?.Count ?? 0)
            .ToList();

        var assigned = new List<Guid>();
        int idx = 0;
        foreach (var task in tasks)
        {
            if (orderedUserIds.Count == 0) break;
            var userId = orderedUserIds[idx % orderedUserIds.Count];
            task.AssignTo(userId, OperationAgentType.Human);
            assigned.Add(task.Id);
            idx++;
        }

        await _context.SaveChangesAsync(cancellationToken);
        return new AssignResult { AssignedCount = assigned.Count, AssignedTaskIds = assigned };
    }

    private static WorkTaskDto MapToDto(WorkTask t, string? warehouseName)
    {
        return new WorkTaskDto
        {
            Id = t.Id,
            WarehouseId = t.WarehouseId,
            WarehouseName = warehouseName ?? "",
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
        };
    }
}
