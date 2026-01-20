using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Warehouses.Models;

namespace Khidmah_Inventory.Application.Features.Warehouses.Queries.GetWarehouse;

public class GetWarehouseQueryHandler : IRequestHandler<GetWarehouseQuery, Result<WarehouseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWarehouseQueryHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WarehouseDto>> Handle(GetWarehouseQuery request, CancellationToken cancellationToken)
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

        var zoneCount = await _context.WarehouseZones
            .CountAsync(z => z.WarehouseId == request.Id && z.CompanyId == companyId.Value && !z.IsDeleted, cancellationToken);

        var dto = new WarehouseDto
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

        return Result<WarehouseDto>.Success(dto);
    }
}

