using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateMapRack;

public class CreateMapRackCommandHandler : IRequestHandler<CreateMapRackCommand, Result<MapRackDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateMapRackCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MapRackDto>> Handle(CreateMapRackCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<MapRackDto>.Failure("Company context is required.");

        var aisle = await _context.MapAisles
            .FirstOrDefaultAsync(a => a.Id == request.MapAisleId && a.CompanyId == companyId.Value && !a.IsDeleted, cancellationToken);
        if (aisle == null)
            return Result<MapRackDto>.Failure("Aisle not found.");

        var rack = new MapRack(companyId.Value, request.MapAisleId, request.Name, request.Code, request.DisplayOrder, _currentUser.UserId);
        _context.MapRacks.Add(rack);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new MapRackDto
        {
            Id = rack.Id,
            MapAisleId = rack.MapAisleId,
            Name = rack.Name,
            Code = rack.Code,
            DisplayOrder = rack.DisplayOrder,
            BinCount = 0
        };
        return Result<MapRackDto>.Success(dto);
    }
}
