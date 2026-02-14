using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ApiKeys.Commands.CreateApiKey;

public class CreateApiKeyCommand : IRequest<Result<CreateApiKeyResult>>
{
    public string Name { get; set; } = string.Empty;
    public string Permissions { get; set; } = string.Empty;
    public DateTime? ExpiresAt { get; set; }
}

public class CreateApiKeyResult
{
    public ApiKeyDto ApiKey { get; set; } = null!;
    /// <summary>Shown only once at creation.</summary>
    public string PlainKey { get; set; } = string.Empty;
}
