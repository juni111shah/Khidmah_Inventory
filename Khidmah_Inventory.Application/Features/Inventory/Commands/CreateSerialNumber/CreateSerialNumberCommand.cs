using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.CreateSerialNumber;

public class CreateSerialNumberCommand : IRequest<Result<SerialNumberDto>>
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public string SerialNumberValue { get; set; } = string.Empty;
    public Guid? BatchId { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? WarrantyExpiryDate { get; set; }
    public string? Notes { get; set; }
}

