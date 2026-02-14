using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ApiKeys.Queries.GetApiKeyUsage;

public class GetApiKeyUsageQuery : IRequest<Result<ApiKeyUsageDto>>
{
    public Guid? ApiKeyId { get; set; }
    public int RecentLogsCount { get; set; } = 50;
}
