using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetWarehouseMapsList;

public class GetWarehouseMapsListQueryHandler : IRequestHandler<GetWarehouseMapsListQuery, Result<List<WarehouseMapDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWarehouseMapsListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<WarehouseMapDto>>> Handle(GetWarehouseMapsListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<WarehouseMapDto>>.Failure("Company context is required.");

        var query = _context.WarehouseMaps
            .Where(m => m.CompanyId == companyId.Value && !m.IsDeleted)
            .AsQueryable();

        if (request.WarehouseId.HasValue)
            query = query.Where(m => m.WarehouseId == request.WarehouseId.Value);
        if (request.IsActive.HasValue)
            query = query.Where(m => m.IsActive == request.IsActive.Value);

        var maps = await query
            .OrderBy(m => m.Name)
            .Include(m => m.Warehouse)
            .ToListAsync(cancellationToken);

        var list = new List<WarehouseMapDto>();
        foreach (var m in maps)
        {
            var zoneCount = await _context.MapZones.CountAsync(z => z.WarehouseMapId == m.Id && !z.IsDeleted, cancellationToken);
            list.Add(new WarehouseMapDto
            {
                Id = m.Id,
                WarehouseId = m.WarehouseId,
                WarehouseName = m.Warehouse?.Name ?? "",
                Name = m.Name,
                Description = m.Description,
                IsActive = m.IsActive,
                ZoneCount = zoneCount,
                CreatedAt = m.CreatedAt,
                UpdatedAt = m.UpdatedAt
            });
        }

        return Result<List<WarehouseMapDto>>.Success(list);
    }
}
