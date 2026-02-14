using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapZone;

public class UpdateMapZoneCommandHandler : IRequestHandler<UpdateMapZoneCommand, Result<MapZoneDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateMapZoneCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MapZoneDto>> Handle(UpdateMapZoneCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<MapZoneDto>.Failure("Company context is required.");

        var zone = await _context.MapZones
            .FirstOrDefaultAsync(z => z.Id == request.Id && z.CompanyId == companyId.Value && !z.IsDeleted, cancellationToken);
        if (zone == null)
            return Result<MapZoneDto>.Failure("Zone not found.");

        zone.Update(request.Name, request.Code, request.DisplayOrder, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        var aisleCount = await _context.MapAisles.CountAsync(a => a.MapZoneId == zone.Id && !a.IsDeleted, cancellationToken);
        var dto = new MapZoneDto
        {
            Id = zone.Id,
            WarehouseMapId = zone.WarehouseMapId,
            Name = zone.Name,
            Code = zone.Code,
            DisplayOrder = zone.DisplayOrder,
            AisleCount = aisleCount
        };
        return Result<MapZoneDto>.Success(dto);
    }
}
