using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.DeleteWarehouseMap;

public class DeleteWarehouseMapCommandHandler : IRequestHandler<DeleteWarehouseMapCommand, Result>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public DeleteWarehouseMapCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result> Handle(DeleteWarehouseMapCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result.Failure("Company context is required.");

        var map = await _context.WarehouseMaps
            .FirstOrDefaultAsync(m => m.Id == request.Id && m.CompanyId == companyId.Value && !m.IsDeleted, cancellationToken);
        if (map == null)
            return Result.Failure("Warehouse map not found.");

        map.MarkAsDeleted(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
