using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateMapBin;

public class CreateMapBinCommandHandler : IRequestHandler<CreateMapBinCommand, Result<MapBinDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateMapBinCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MapBinDto>> Handle(CreateMapBinCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<MapBinDto>.Failure("Company context is required.");

        var rack = await _context.MapRacks
            .FirstOrDefaultAsync(r => r.Id == request.MapRackId && r.CompanyId == companyId.Value && !r.IsDeleted, cancellationToken);
        if (rack == null)
            return Result<MapBinDto>.Failure("Rack not found.");

        var bin = new MapBin(companyId.Value, request.MapRackId, request.Name, request.X, request.Y,
            request.Code, request.BinId, request.DisplayOrder, _currentUser.UserId);
        _context.MapBins.Add(bin);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new MapBinDto
        {
            Id = bin.Id,
            MapRackId = bin.MapRackId,
            Name = bin.Name,
            Code = bin.Code,
            X = bin.X,
            Y = bin.Y,
            DisplayOrder = bin.DisplayOrder,
            BinId = bin.BinId
        };
        return Result<MapBinDto>.Success(dto);
    }
}
