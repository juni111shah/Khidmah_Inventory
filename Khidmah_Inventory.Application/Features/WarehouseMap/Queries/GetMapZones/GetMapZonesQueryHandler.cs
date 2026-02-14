using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapZones;

public class GetMapZonesQueryHandler : IRequestHandler<GetMapZonesQuery, Result<List<MapZoneDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMapZonesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<MapZoneDto>>> Handle(GetMapZonesQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<MapZoneDto>>.Failure("Company context is required.");

        var zones = await _context.MapZones
            .Where(z => z.WarehouseMapId == request.WarehouseMapId && z.CompanyId == companyId.Value && !z.IsDeleted)
            .OrderBy(z => z.DisplayOrder).ThenBy(z => z.Name)
            .ToListAsync(cancellationToken);

        var list = new List<MapZoneDto>();
        foreach (var z in zones)
        {
            var aisleCount = await _context.MapAisles.CountAsync(a => a.MapZoneId == z.Id && !a.IsDeleted, cancellationToken);
            list.Add(new MapZoneDto
            {
                Id = z.Id,
                WarehouseMapId = z.WarehouseMapId,
                Name = z.Name,
                Code = z.Code,
                DisplayOrder = z.DisplayOrder,
                AisleCount = aisleCount
            });
        }
        return Result<List<MapZoneDto>>.Success(list);
    }
}
