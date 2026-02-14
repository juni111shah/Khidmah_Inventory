using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Features.AutonomousWarehouse.Models;
using Khidmah_Inventory.Domain.Enums;

namespace Khidmah_Inventory.Infrastructure.Services;

/// <summary>
/// Nearest-neighbor route optimizer. Can be replaced with AI/ML implementation later.
/// </summary>
public class RouteOptimizerService : IRouteOptimizer
{
    private readonly IApplicationDbContext _context;

    public RouteOptimizerService(IApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<OptimizedRouteResult> GetOptimalSequenceAsync(
        Guid companyId,
        Guid warehouseId,
        RouteOptimizerRequest request,
        CancellationToken cancellationToken = default)
    {
        decimal currentX = request.CurrentX ?? 0;
        decimal currentY = request.CurrentY ?? 0;
        if (request.StartMapBinId.HasValue)
        {
            var startBin = await _context.MapBins
                .FirstOrDefaultAsync(b => b.Id == request.StartMapBinId && b.CompanyId == companyId && !b.IsDeleted, cancellationToken);
            if (startBin != null)
            {
                currentX = startBin.X;
                currentY = startBin.Y;
            }
        }

        if (request.TaskIds == null || request.TaskIds.Count == 0)
            return new OptimizedRouteResult { OrderedTaskIds = new List<Guid>(), EstimatedTotalDistance = 0 };

        var tasks = await _context.WorkTasks
            .Include(t => t.MapBin)
            .Where(t => t.CompanyId == companyId && request.TaskIds.Contains(t.Id) && !t.IsDeleted)
            .ToListAsync(cancellationToken);

        var withPosition = new List<(Guid TaskId, decimal X, decimal Y)>();
        foreach (var t in tasks)
        {
            if (t.MapBinId.HasValue && t.MapBin != null)
            {
                withPosition.Add((t.Id, t.MapBin.X, t.MapBin.Y));
            }
            else
            {
                withPosition.Add((t.Id, currentX, currentY));
            }
        }

        var ordered = new List<Guid>();
        decimal totalDist = 0;
        var remaining = withPosition.ToList();
        var x = currentX;
        var y = currentY;

        while (remaining.Count > 0)
        {
            var nearest = remaining
                .Select(p => (p, d: Distance(x, y, p.X, p.Y)))
                .OrderBy(x => x.d)
                .First();
            ordered.Add(nearest.p.TaskId);
            totalDist += nearest.d;
            x = nearest.p.X;
            y = nearest.p.Y;
            remaining.Remove(nearest.p);
        }

        return new OptimizedRouteResult
        {
            OrderedTaskIds = ordered,
            EstimatedTotalDistance = totalDist
        };
    }

    private static decimal Distance(decimal x1, decimal y1, decimal x2, decimal y2)
    {
        var dx = x2 - x1;
        var dy = y2 - y1;
        return (decimal)Math.Sqrt((double)(dx * dx + dy * dy));
    }
}
