using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ApiKeys.Commands.RevokeApiKey;

public class RevokeApiKeyCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
