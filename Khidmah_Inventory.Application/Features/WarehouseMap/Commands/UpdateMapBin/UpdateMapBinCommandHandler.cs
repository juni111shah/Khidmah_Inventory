using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.UpdateMapBin;

public class UpdateMapBinCommandHandler : IRequestHandler<UpdateMapBinCommand, Result<MapBinDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateMapBinCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<MapBinDto>> Handle(UpdateMapBinCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<MapBinDto>.Failure("Company context is required.");

        var bin = await _context.MapBins
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == companyId.Value && !b.IsDeleted, cancellationToken);
        if (bin == null)
            return Result<MapBinDto>.Failure("Bin not found.");

        bin.Update(request.Name, request.X, request.Y, request.Code, request.BinId, request.DisplayOrder, _currentUser.UserId);
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
