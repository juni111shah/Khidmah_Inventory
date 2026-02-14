using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateMapAisle;

public class CreateMapAisleCommandHandler : IRequestHandler<CreateMapAisleCommand, Result<MapAisleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateMapAisleCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MapAisleDto>> Handle(CreateMapAisleCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<MapAisleDto>.Failure("Company context is required.");

        var zone = await _context.MapZones
            .FirstOrDefaultAsync(z => z.Id == request.MapZoneId && z.CompanyId == companyId.Value && !z.IsDeleted, cancellationToken);
        if (zone == null)
            return Result<MapAisleDto>.Failure("Zone not found.");

        var aisle = new MapAisle(companyId.Value, request.MapZoneId, request.Name, request.Code, request.DisplayOrder, _currentUser.UserId);
        _context.MapAisles.Add(aisle);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new MapAisleDto
        {
            Id = aisle.Id,
            MapZoneId = aisle.MapZoneId,
            Name = aisle.Name,
            Code = aisle.Code,
            DisplayOrder = aisle.DisplayOrder,
            RackCount = 0
        };
        return Result<MapAisleDto>.Success(dto);
    }
}
