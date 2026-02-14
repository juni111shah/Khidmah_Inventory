using MediatR;
using Khidmah_Inventory.Application.Common.Constants;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Notifications.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Notifications.Commands.CreateNotification;

public class CreateNotificationCommandHandler : IRequestHandler<CreateNotificationCommand, Result<NotificationDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly IOperationsBroadcast? _broadcast;

    public CreateNotificationCommandHandler(IApplicationDbContext context, IOperationsBroadcast? broadcast = null)
    {
        _context = context;
        _broadcast = broadcast;
    }

    public async Task<Result<NotificationDto>> Handle(CreateNotificationCommand request, CancellationToken cancellationToken)
    {
        var notification = new Notification(
            request.CompanyId,
            request.Title,
            request.Message,
            request.Type,
            request.UserId,
            request.EntityType,
            request.EntityId);

        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);

        var dto = new NotificationDto
        {
            Id = notification.Id,
            CompanyId = notification.CompanyId,
            UserId = notification.UserId,
            Title = notification.Title,
            Message = notification.Message,
            Type = notification.Type,
            EntityType = notification.EntityType,
            EntityId = notification.EntityId,
            IsRead = notification.IsRead,
            CreatedAt = notification.CreatedAt
        };

        if (_broadcast != null)
        {
            await _broadcast.BroadcastAsync(
                OperationsEventNames.NotificationRaised,
                request.CompanyId,
                request.EntityId,
                request.EntityType,
                dto,
                cancellationToken);
        }

        return Result<NotificationDto>.Success(dto);
    }
}
