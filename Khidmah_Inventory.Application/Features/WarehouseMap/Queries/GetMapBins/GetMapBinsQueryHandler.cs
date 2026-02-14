using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Queries.GetMapBins;

public class GetMapBinsQueryHandler : IRequestHandler<GetMapBinsQuery, Result<List<MapBinDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetMapBinsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<MapBinDto>>> Handle(GetMapBinsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<MapBinDto>>.Failure("Company context is required.");

        var bins = await _context.MapBins
            .Where(b => b.MapRackId == request.MapRackId && b.CompanyId == companyId.Value && !b.IsDeleted)
            .OrderBy(b => b.DisplayOrder).ThenBy(b => b.Name)
            .ToListAsync(cancellationToken);

        var list = bins.Select(b => new MapBinDto
        {
            Id = b.Id,
            MapRackId = b.MapRackId,
            Name = b.Name,
            Code = b.Code,
            X = b.X,
            Y = b.Y,
            DisplayOrder = b.DisplayOrder,
            BinId = b.BinId
        }).ToList();
        return Result<List<MapBinDto>>.Success(list);
    }
}
