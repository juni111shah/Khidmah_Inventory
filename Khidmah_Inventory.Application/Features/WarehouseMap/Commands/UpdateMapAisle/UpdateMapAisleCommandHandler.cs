using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapAisle;

public class UpdateMapAisleCommandHandler : IRequestHandler<UpdateMapAisleCommand, Result<MapAisleDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateMapAisleCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MapAisleDto>> Handle(UpdateMapAisleCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<MapAisleDto>.Failure("Company context is required.");

        var aisle = await _context.MapAisles
            .FirstOrDefaultAsync(a => a.Id == request.Id && a.CompanyId == companyId.Value && !a.IsDeleted, cancellationToken);
        if (aisle == null)
            return Result<MapAisleDto>.Failure("Aisle not found.");

        aisle.Update(request.Name, request.Code, request.DisplayOrder, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        var rackCount = await _context.MapRacks.CountAsync(r => r.MapAisleId == aisle.Id && !r.IsDeleted, cancellationToken);
        var dto = new MapAisleDto
        {
            Id = aisle.Id,
            MapZoneId = aisle.MapZoneId,
            Name = aisle.Name,
            Code = aisle.Code,
            DisplayOrder = aisle.DisplayOrder,
            RackCount = rackCount
        };
        return Result<MapAisleDto>.Success(dto);
    }
}
