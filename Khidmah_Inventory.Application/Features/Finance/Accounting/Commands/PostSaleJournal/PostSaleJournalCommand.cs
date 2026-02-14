using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Accounting.Commands.PostSaleJournal;

public class PostSaleJournalCommand : IRequest<Result<Guid>>
{
    public Guid CompanyId { get; set; }
    public Guid SourceId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public bool IsCashSale { get; set; }
}
