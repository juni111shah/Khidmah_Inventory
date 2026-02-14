using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Finance.Accounting.Commands.PostAdjustmentJournal;

public class PostAdjustmentJournalCommand : IRequest<Result<Guid>>
{
    public Guid CompanyId { get; set; }
    public Guid? SourceId { get; set; }
    public string Reference { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public decimal LossAmount { get; set; }
    public string SourceModule { get; set; } = JournalSourceModule.Adjustment;
}
