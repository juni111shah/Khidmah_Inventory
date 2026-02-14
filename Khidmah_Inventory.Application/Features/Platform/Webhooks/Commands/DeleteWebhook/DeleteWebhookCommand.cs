using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Commands.DeleteWebhook;

public class DeleteWebhookCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
