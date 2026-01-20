using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Warehouses.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Warehouses.Commands.CreateWarehouse;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, Result<WarehouseDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateWarehouseCommandHandler(
        IApplicationDbContext context,
        ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WarehouseDto>> Handle(CreateWarehouseCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
        {
            return Result<WarehouseDto>.Failure("Company context is required");
        }

        // If this is set as default, unset other default warehouses
        if (request.IsDefault)
        {
            var existingDefaults = await _context.Warehouses
                .Where(w => w.CompanyId == companyId.Value && w.IsDefault && !w.IsDeleted)
                .ToListAsync(cancellationToken);

            foreach (var existingDefault in existingDefaults)
            {
                existingDefault.RemoveDefault();
            }
        }

        var warehouse = new Warehouse(
            companyId.Value,
            request.Name,
            request.Code,
            _currentUser.UserId);

        warehouse.Update(
            request.Name,
            request.Code,
            request.Description,
            request.Address,
            request.City,
            request.State,
            request.Country,
            request.PostalCode,
            request.PhoneNumber,
            request.Email,
            _currentUser.UserId);

        if (request.IsDefault)
        {
            warehouse.SetAsDefault();
        }

        _context.Warehouses.Add(warehouse);
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
            throw new InvalidOperationException("Warehouse not found after creation");
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

