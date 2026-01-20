using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.CreateSerialNumber;

public class CreateSerialNumberCommandHandler : IRequestHandler<CreateSerialNumberCommand, Result<SerialNumberDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateSerialNumberCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<SerialNumberDto>> Handle(CreateSerialNumberCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<SerialNumberDto>.Failure("Company context is required");

        // Validate product
        var product = await _context.Products
            .FirstOrDefaultAsync(p => p.Id == request.ProductId && p.CompanyId == companyId.Value && !p.IsDeleted, cancellationToken);

        if (product == null)
            return Result<SerialNumberDto>.Failure("Product not found.");

        // Validate warehouse
        var warehouse = await _context.Warehouses
            .FirstOrDefaultAsync(w => w.Id == request.WarehouseId && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);

        if (warehouse == null)
            return Result<SerialNumberDto>.Failure("Warehouse not found.");

        // Check if serial number already exists
        var existingSerial = await _context.SerialNumbers
            .FirstOrDefaultAsync(s => s.CompanyId == companyId.Value && 
                s.SerialNumberValue == request.SerialNumberValue && 
                !s.IsDeleted, cancellationToken);

        if (existingSerial != null)
            return Result<SerialNumberDto>.Failure("Serial number already exists.");

        // Validate batch if provided
        Batch? batch = null;
        if (request.BatchId.HasValue)
        {
            batch = await _context.Batches
                .FirstOrDefaultAsync(b => b.Id == request.BatchId.Value && 
                    b.CompanyId == companyId.Value && 
                    !b.IsDeleted, cancellationToken);

            if (batch == null)
                return Result<SerialNumberDto>.Failure("Batch not found.");
        }

        // Create serial number
        var serialNumber = new SerialNumber(companyId.Value, request.ProductId, request.WarehouseId, request.SerialNumberValue, _currentUser.UserId);

        if (request.BatchId.HasValue && batch != null)
            serialNumber.AssignToBatch(request.BatchId.Value, request.BatchNumber ?? batch.BatchNumber);

        if (request.ManufactureDate.HasValue)
            serialNumber.SetManufactureDate(request.ManufactureDate);

        if (request.ExpiryDate.HasValue)
            serialNumber.SetExpiryDate(request.ExpiryDate);

        if (!string.IsNullOrWhiteSpace(request.WarrantyExpiryDate))
            serialNumber.SetWarrantyExpiry(request.WarrantyExpiryDate);

        _context.SerialNumbers.Add(serialNumber);
        await _context.SaveChangesAsync(cancellationToken);

        // Load with navigation properties for DTO mapping
        var serialWithNav = await _context.SerialNumbers
            .Include(s => s.Product)
            .Include(s => s.Warehouse)
            .Include(s => s.Batch)
            .Include(s => s.SalesOrder)
            .Include(s => s.Customer)
            .FirstOrDefaultAsync(s => s.Id == serialNumber.Id && s.CompanyId == companyId.Value, cancellationToken);

        var dto = new SerialNumberDto
        {
            Id = serialWithNav!.Id,
            ProductId = serialWithNav.ProductId,
            ProductName = serialWithNav.Product.Name,
            ProductSKU = serialWithNav.Product.SKU,
            WarehouseId = serialWithNav.WarehouseId,
            WarehouseName = serialWithNav.Warehouse.Name,
            SerialNumberValue = serialWithNav.SerialNumberValue,
            BatchNumber = serialWithNav.BatchNumber,
            BatchId = serialWithNav.BatchId,
            ManufactureDate = serialWithNav.ManufactureDate,
            ExpiryDate = serialWithNav.ExpiryDate,
            Status = serialWithNav.Status,
            SalesOrderId = serialWithNav.SalesOrderId,
            SalesOrderNumber = serialWithNav.SalesOrder?.OrderNumber,
            CustomerId = serialWithNav.CustomerId,
            CustomerName = serialWithNav.Customer?.Name,
            SoldDate = serialWithNav.SoldDate,
            WarrantyExpiryDate = serialWithNav.WarrantyExpiryDate,
            Notes = serialWithNav.Notes,
            IsRecalled = serialWithNav.IsRecalled,
            RecallDate = serialWithNav.RecallDate,
            RecallReason = serialWithNav.RecallReason,
            IsExpired = serialWithNav.IsExpired,
            IsExpiringSoon = serialWithNav.IsExpiringSoon,
            CreatedAt = serialWithNav.CreatedAt
        };

        return Result<SerialNumberDto>.Success(dto);
    }
}

