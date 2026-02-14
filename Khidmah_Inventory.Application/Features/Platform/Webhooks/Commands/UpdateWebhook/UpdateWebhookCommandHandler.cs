using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Commands.UpdateWebhook;

public class UpdateWebhookCommandHandler : IRequestHandler<UpdateWebhookCommand, Result<WebhookDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public UpdateWebhookCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WebhookDto>> Handle(UpdateWebhookCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<WebhookDto>.Failure("Company context is required");

        var webhook = await _context.Webhooks
            .FirstOrDefaultAsync(w => w.Id == request.Id && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);
        if (webhook == null)
            return Result<WebhookDto>.Failure("Webhook not found");

        webhook.Update(request.Name.Trim(), request.Url.Trim(), request.Events?.Trim() ?? "", request.Secret?.Trim(), request.Description?.Trim(), request.IsActive, _currentUser.UserId);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<WebhookDto>.Success(new WebhookDto
        {
            Id = webhook.Id,
            Name = webhook.Name,
            Url = webhook.Url,
            Events = webhook.Events,
            IsActive = webhook.IsActive,
            Description = webhook.Description,
            CreatedAt = webhook.CreatedAt
        });
    }
}
