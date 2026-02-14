using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Finance.Accounts.Models;

namespace Khidmah_Inventory.Application.Features.Finance.Accounts.Commands.ImportStandardChart;

public class ImportStandardChartCommand : IRequest<Result<ImportStandardChartResult>>
{
}

public class ImportStandardChartResult
{
    public int Created { get; set; }
    public int Skipped { get; set; }
    public List<AccountDto> Accounts { get; set; } = new();
}
