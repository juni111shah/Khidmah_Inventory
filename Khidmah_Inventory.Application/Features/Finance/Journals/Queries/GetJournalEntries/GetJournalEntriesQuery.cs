using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Journals.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Journals.Queries.GetJournalEntries;

public class GetJournalEntriesQuery : IRequest<Result<GetJournalEntriesResult>>
{
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public string? SourceModule { get; set; }
    public int PageNo { get; set; } = 1;
    public int PageSize { get; set; } = 20;
}

public class GetJournalEntriesResult
{
    public List<JournalEntryDto> Items { get; set; } = new();
    public int TotalCount { get; set; }
    public int PageNo { get; set; }
    public int PageSize { get; set; }
}
