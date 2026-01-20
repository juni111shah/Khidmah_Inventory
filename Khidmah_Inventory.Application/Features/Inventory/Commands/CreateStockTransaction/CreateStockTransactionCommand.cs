using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;

namespace Khidmah_Inventory.Application.Features.Inventory.Commands.CreateStockTransaction;

public class CreateStockTransactionCommand : IRequest<Result<StockTransactionDto>>
{
    public Guid ProductId { get; set; }
    public Guid WarehouseId { get; set; }
    public string TransactionType { get; set; } = string.Empty; // StockIn, StockOut, Adjustment, Transfer
    public decimal Quantity { get; set; }
    public decimal? UnitCost { get; set; }
    public string? ReferenceType { get; set; }
    public Guid? ReferenceId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? BatchNumber { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public string? Notes { get; set; }
    public DateTime? TransactionDate { get; set; }
}

