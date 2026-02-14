using MediatR;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Features.Notifications.Commands.MarkNotificationRead;

public class MarkNotificationReadCommand : IRequest<Result>
{
    public Guid Id { get; set; }
}
