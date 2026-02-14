using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetWarehouseMapById;

public class GetWarehouseMapByIdQueryHandler : IRequestHandler<GetWarehouseMapByIdQuery, Result<WarehouseMapTreeDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWarehouseMapByIdQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WarehouseMapTreeDto>> Handle(GetWarehouseMapByIdQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<WarehouseMapTreeDto>.Failure("Company context is required.");

        var map = await _context.WarehouseMaps
            .Include(m => m.Warehouse)
            .Include(m => m.Zones)
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.CompanyId == companyId.Value && !m.IsDeleted, cancellationToken);

        if (map == null)
            return Result<WarehouseMapTreeDto>.Failure("Warehouse map not found.");

        var zoneIds = map.Zones.Where(z => !z.IsDeleted).Select(z => z.Id).ToList();
        var aisles = await _context.MapAisles
            .Where(a => a.CompanyId == companyId.Value && !a.IsDeleted && zoneIds.Contains(a.MapZoneId))
            .OrderBy(a => a.DisplayOrder).ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);
        var aisleIds = aisles.Select(a => a.Id).ToList();
        var racks = await _context.MapRacks
            .Where(r => r.CompanyId == companyId.Value && !r.IsDeleted && aisleIds.Contains(r.MapAisleId))
            .OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);
        var rackIds = racks.Select(r => r.Id).ToList();
        var bins = await _context.MapBins
            .Where(b => b.CompanyId == companyId.Value && !b.IsDeleted && rackIds.Contains(b.MapRackId))
            .OrderBy(b => b.DisplayOrder).ThenBy(b => b.Name)
            .ToListAsync(cancellationToken);

        var mapDto = new WarehouseMapDto
        {
            Id = map.Id,
            WarehouseId = map.WarehouseId,
            WarehouseName = map.Warehouse?.Name ?? "",
            Name = map.Name,
            Description = map.Description,
            IsActive = map.IsActive,
            ZoneCount = map.Zones.Count(z => !z.IsDeleted),
            CreatedAt = map.CreatedAt,
            UpdatedAt = map.UpdatedAt
        };

        var zoneTrees = map.Zones
            .Where(z => !z.IsDeleted)
            .OrderBy(z => z.DisplayOrder).ThenBy(z => z.Name)
            .Select(z =>
            {
                var zoneAisles = aisles.Where(a => a.MapZoneId == z.Id).ToList();
                var aisleTrees = zoneAisles.Select(a =>
                {
                    var aisleRacks = racks.Where(r => r.MapAisleId == a.Id).ToList();
                    var rackTrees = aisleRacks.Select(r => new MapRackTreeDto
                    {
                        Rack = new MapRackDto
                        {
                            Id = r.Id,
                            MapAisleId = r.MapAisleId,
                            Name = r.Name,
                            Code = r.Code,
                            DisplayOrder = r.DisplayOrder,
                            BinCount = bins.Count(b => b.MapRackId == r.Id)
                        },
                        Bins = bins.Where(b => b.MapRackId == r.Id).Select(b => new MapBinDto
                        {
                            Id = b.Id,
                            MapRackId = b.MapRackId,
                            Name = b.Name,
                            Code = b.Code,
                            X = b.X,
                            Y = b.Y,
                            DisplayOrder = b.DisplayOrder,
                            BinId = b.BinId
                        }).OrderBy(b => b.DisplayOrder).ThenBy(b => b.Name).ToList()
                    }).ToList();
                    return new MapAisleTreeDto
                    {
                        Aisle = new MapAisleDto
                        {
                            Id = a.Id,
                            MapZoneId = a.MapZoneId,
                            Name = a.Name,
                            Code = a.Code,
                            DisplayOrder = a.DisplayOrder,
                            RackCount = aisleRacks.Count
                        },
                        Racks = rackTrees
                    };
                }).ToList();
                return new MapZoneTreeDto
                {
                    Zone = new MapZoneDto
                    {
                        Id = z.Id,
                        WarehouseMapId = z.WarehouseMapId,
                        Name = z.Name,
                        Code = z.Code,
                        DisplayOrder = z.DisplayOrder,
                        AisleCount = zoneAisles.Count
                    },
                    Aisles = aisleTrees
                };
            }).ToList();

        var tree = new WarehouseMapTreeDto { Map = mapDto, Zones = zoneTrees };
        return Result<WarehouseMapTreeDto>.Success(tree);
    }
}
