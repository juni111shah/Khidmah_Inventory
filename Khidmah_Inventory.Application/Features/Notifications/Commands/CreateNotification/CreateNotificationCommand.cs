using MediatR;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Notifications.Models;

namespace Khidmah_Inventory.Application.Features.Notifications.Commands.CreateNotification;

/// <summary>
/// Internal use: create a notification (e.g. from PO created, SO created, low stock, workflow approval).
/// </summary>
public class CreateNotificationCommand : IRequest<Result<NotificationDto>>
{
    public Guid CompanyId { get; set; }
    public Guid? UserId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = "Info"; // Info, Success, Warning, Error
    public string? EntityType { get; set; }
    public Guid? EntityId { get; set; }
}
