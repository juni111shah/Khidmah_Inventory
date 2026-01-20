using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Warehouses.Models;

namespace Khidmah_Inventory.Application.Features.Warehouses.Commands.ActivateWarehouse;

public class ActivateWarehouseCommandHandler : IRequestHandler<ActivateWarehouseCommand, Result<WarehouseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public ActivateWarehouseCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WarehouseDto>> Handle(ActivateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<WarehouseDto>.Failure("Company context is required");
        }

        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.Id && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);

        if (warehouse == null)
        {
            return Result<WarehouseDto>.Failure("Warehouse not found.");
        }

        warehouse.Activate(_currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = await MapToDtoAsync(warehouse.Id, companyId.Value, cancellationToken);
        return Result<WarehouseDto>.Success(dto);
    }

    private async Task<WarehouseDto> MapToDtoAsync(Guid warehouseId, Guid companyId, CancellationToken cancellationToken)
    {
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == warehouseId && w.CompanyId == companyId, cancellationToken);

        if (warehouse == null)
        {
            throw new InvalidOperationException("Warehouse not found");
        }

        var zoneCount = await _context.WarehouseZones
            .CountAsync(z => z.WarehouseId == warehouseId && z.CompanyId == companyId && !z.IsDeleted, cancellationToken);

        return new WarehouseDto
        {
            Id = warehouse.Id,
            Name = warehouse.Name,
            Code = warehouse.Code,
            Description = warehouse.Description,
            Address = warehouse.Address,
            City = warehouse.City,
            State = warehouse.State,
            Country = warehouse.Country,
            PostalCode = warehouse.PostalCode,
            PhoneNumber = warehouse.PhoneNumber,
            Email = warehouse.Email,
            IsDefault = warehouse.IsDefault,
            IsActive = warehouse.IsActive,
            ZoneCount = zoneCount,
            CreatedAt = warehouse.CreatedAt,
            UpdatedAt = warehouse.UpdatedAt
        };
    }
}

