using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Inventory.Models;

namespace Khidmah_Inventory.Application.Features.Inventory.Queries.GetStockTransactionsList;

public class GetStockTransactionsListQuery : IRequest<Result<PagedResult<StockTransactionDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public Guid? ProductId { get; set; }
    public Guid? WarehouseId { get; set; }
    public string? TransactionType { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

