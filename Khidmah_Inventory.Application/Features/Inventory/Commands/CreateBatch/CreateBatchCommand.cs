using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.CreateBatch;

public class CreateBatchCommand : IRequest<Result<BatchDto>>
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public string BatchNumber { get; set; } = string.Empty;
    public string? LotNumber { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? SupplierName { get; set; }
    public string? SupplierBatchNumber { get; set; }
    public string? Notes { get; set; }
}

