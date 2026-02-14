using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateWarehouseMap;

public class UpdateWarehouseMapCommandHandler : IRequestHandler<UpdateWarehouseMapCommand, Result<WarehouseMapDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateWarehouseMapCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WarehouseMapDto>> Handle(UpdateWarehouseMapCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<WarehouseMapDto>.Failure("Company context is required.");

        var map = await _context.WarehouseMaps
            .Include(m => m.Warehouse)
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.CompanyId == companyId.Value && !m.IsDeleted, cancellationToken);
        if (map == null)
            return Result<WarehouseMapDto>.Failure("Warehouse map not found.");

        map.Update(request.Name, request.Description, _currentUser.UserId);
        map.SetActive(request.IsActive, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        var zoneCount = await _context.MapZones.CountAsync(z => z.WarehouseMapId == map.Id && !z.IsDeleted, cancellationToken);
        var dto = new WarehouseMapDto
        {
            Id = map.Id,
            WarehouseId = map.WarehouseId,
            WarehouseName = map.Warehouse?.Name ?? "",
            Name = map.Name,
            Description = map.Description,
            IsActive = map.IsActive,
            ZoneCount = zoneCount,
            CreatedAt = map.CreatedAt,
            UpdatedAt = map.UpdatedAt
        };
        return Result<WarehouseMapDto>.Success(dto);
    }
}
