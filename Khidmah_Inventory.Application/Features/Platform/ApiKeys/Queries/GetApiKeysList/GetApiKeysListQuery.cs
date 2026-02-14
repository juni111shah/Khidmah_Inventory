using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ApiKeys.Queries.GetApiKeysList;

public class GetApiKeysListQuery : IRequest<Result<PagedResult<ApiKeyDto>>>
{
    public FilterRequest? FilterRequest { get; set; }
    public bool? IsActive { get; set; }
}
