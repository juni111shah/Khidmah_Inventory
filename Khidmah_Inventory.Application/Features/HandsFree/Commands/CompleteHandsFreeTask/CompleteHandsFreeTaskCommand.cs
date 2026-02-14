using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.HandsFree.Commands.CompleteHandsFreeTask;

public class CompleteHandsFreeTaskCommand : IRequest<Result>
{
    public Guid TaskId { get; set; }
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public string BarcodeScanned { get; set; } = string.Empty;
    public decimal QuantityPicked { get; set; }
}
