using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Integrations.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Integrations.Commands.ToggleIntegration;

public class ToggleIntegrationCommand : IRequest<Result<IntegrationDto>>
{
    public string IntegrationType { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
}
