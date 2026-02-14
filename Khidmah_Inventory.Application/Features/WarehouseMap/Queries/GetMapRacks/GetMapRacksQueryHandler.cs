using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapRacks;

public class GetMapRacksQueryHandler : IRequestHandler<GetMapRacksQuery, Result<List<MapRackDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMapRacksQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<MapRackDto>>> Handle(GetMapRacksQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<MapRackDto>>.Failure("Company context is required.");

        var racks = await _context.MapRacks
            .Where(r => r.MapAisleId == request.MapAisleId && r.CompanyId == companyId.Value && !r.IsDeleted)
            .OrderBy(r => r.DisplayOrder).ThenBy(r => r.Name)
            .ToListAsync(cancellationToken);

        var list = new List<MapRackDto>();
        foreach (var r in racks)
        {
            var binCount = await _context.MapBins.CountAsync(b => b.MapRackId == r.Id && !b.IsDeleted, cancellationToken);
            list.Add(new MapRackDto
            {
                Id = r.Id,
                MapAisleId = r.MapAisleId,
                Name = r.Name,
                Code = r.Code,
                DisplayOrder = r.DisplayOrder,
                BinCount = binCount
            });
        }
        return Result<List<MapRackDto>>.Success(list);
    }
}
