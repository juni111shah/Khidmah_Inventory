using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateMapZone;

public class CreateMapZoneCommandHandler : IRequestHandler<CreateMapZoneCommand, Result<MapZoneDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateMapZoneCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MapZoneDto>> Handle(CreateMapZoneCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<MapZoneDto>.Failure("Company context is required.");

        var map = await _context.WarehouseMaps
            .FirstOrDefaultAsync(m => m.Id == request.WarehouseMapId && m.CompanyId == companyId.Value && !m.IsDeleted, cancellationToken);
        if (map == null)
            return Result<MapZoneDto>.Failure("Warehouse map not found.");

        var zone = new MapZone(companyId.Value, request.WarehouseMapId, request.Name, request.Code, request.DisplayOrder, _currentUser.UserId);
        _context.MapZones.Add(zone);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new MapZoneDto
        {
            Id = zone.Id,
            WarehouseMapId = zone.WarehouseMapId,
            Name = zone.Name,
            Code = zone.Code,
            DisplayOrder = zone.DisplayOrder,
            AisleCount = 0
        };
        return Result<MapZoneDto>.Success(dto);
    }
}
