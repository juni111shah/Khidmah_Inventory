using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapAisles;

public class GetMapAislesQueryHandler : IRequestHandler<GetMapAislesQuery, Result<List<MapAisleDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMapAislesQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<MapAisleDto>>> Handle(GetMapAislesQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<MapAisleDto>>.Failure("Company context is required.");

        var aisles = await _context.MapAisles
            .Where(a => a.MapZoneId == request.MapZoneId && a.CompanyId == companyId.Value && !a.IsDeleted)
            .OrderBy(a => a.DisplayOrder).ThenBy(a => a.Name)
            .ToListAsync(cancellationToken);

        var list = new List<MapAisleDto>();
        foreach (var a in aisles)
        {
            var rackCount = await _context.MapRacks.CountAsync(r => r.MapAisleId == a.Id && !r.IsDeleted, cancellationToken);
            list.Add(new MapAisleDto
            {
                Id = a.Id,
                MapZoneId = a.MapZoneId,
                Name = a.Name,
                Code = a.Code,
                DisplayOrder = a.DisplayOrder,
                RackCount = rackCount
            });
        }
        return Result<List<MapAisleDto>>.Success(list);
    }
}
