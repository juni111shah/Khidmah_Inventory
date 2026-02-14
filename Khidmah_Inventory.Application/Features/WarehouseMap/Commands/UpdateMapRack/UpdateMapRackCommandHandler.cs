using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapRack;

public class UpdateMapRackCommandHandler : IRequestHandler<UpdateMapRackCommand, Result<MapRackDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateMapRackCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MapRackDto>> Handle(UpdateMapRackCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<MapRackDto>.Failure("Company context is required.");

        var rack = await _context.MapRacks
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == companyId.Value && !r.IsDeleted, cancellationToken);
        if (rack == null)
            return Result<MapRackDto>.Failure("Rack not found.");

        rack.Update(request.Name, request.Code, request.DisplayOrder, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        var binCount = await _context.MapBins.CountAsync(b => b.MapRackId == rack.Id && !b.IsDeleted, cancellationToken);
        var dto = new MapRackDto
        {
            Id = rack.Id,
            MapAisleId = rack.MapAisleId,
            Name = rack.Name,
            Code = rack.Code,
            DisplayOrder = rack.DisplayOrder,
            BinCount = binCount
        };
        return Result<MapRackDto>.Success(dto);
    }
}
