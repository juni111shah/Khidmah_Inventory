using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Integrations.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Integrations.Queries.GetIntegrationsList;

public class GetIntegrationsListQuery : IRequest<Result<List<IntegrationDto>>>
{
}
