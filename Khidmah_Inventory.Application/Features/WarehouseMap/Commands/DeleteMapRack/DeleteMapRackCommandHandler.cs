using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteMapRack;

public class DeleteMapRackCommandHandler : IRequestHandler<DeleteMapRackCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteMapRackCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteMapRackCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required.");

        var rack = await _context.MapRacks
            .FirstOrDefaultAsync(r => r.Id == request.Id && r.CompanyId == companyId.Value && !r.IsDeleted, cancellationToken);
        if (rack == null)
            return Result.Failure("Rack not found.");

        rack.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
