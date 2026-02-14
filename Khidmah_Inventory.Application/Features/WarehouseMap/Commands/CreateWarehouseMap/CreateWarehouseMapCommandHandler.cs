using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.WarehouseMap.Models;

namespace Khidmah_Inventory.Application.Features.WarehouseMap.Commands.CreateWarehouseMap;

public class CreateWarehouseMapCommandHandler : IRequestHandler<CreateWarehouseMapCommand, Result<WarehouseMapDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateWarehouseMapCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WarehouseMapDto>> Handle(CreateWarehouseMapCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<WarehouseMapDto>.Failure("Company context is required.");

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.WarehouseId && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);
        if (warehouse == null)
            return Result<WarehouseMapDto>.Failure("Warehouse not found.");

        var map = new Domain.Entities.WarehouseMap(companyId.Value, request.WarehouseId, request.Name, request.Description, _currentUser.UserId);
        _context.WarehouseMaps.Add(map);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new WarehouseMapDto
        {
            Id = map.Id,
            WarehouseId = map.WarehouseId,
            WarehouseName = warehouse.Name,
            Name = map.Name,
            Description = map.Description,
            IsActive = map.IsActive,
            ZoneCount = 0,
            CreatedAt = map.CreatedAt,
            UpdatedAt = map.UpdatedAt
        };
        return Result<WarehouseMapDto>.Success(dto);
    }
}
