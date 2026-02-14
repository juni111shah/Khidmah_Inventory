using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapBin;

public class DeleteMapBinCommandHandler : IRequestHandler<DeleteMapBinCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteMapBinCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteMapBinCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required.");

        var bin = await _context.MapBins
            .FirstOrDefaultAsync(b => b.Id == request.Id && b.CompanyId == companyId.Value && !b.IsDeleted, cancellationToken);
        if (bin == null)
            return Result.Failure("Bin not found.");

        bin.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
