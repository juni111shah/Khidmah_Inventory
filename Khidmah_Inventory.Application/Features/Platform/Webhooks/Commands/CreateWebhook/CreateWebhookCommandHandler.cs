using MediatR;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Models;
using Khidmah_Inventory.Domain.Entities;

namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Commands.CreateWebhook;

public class CreateWebhookCommandHandler : IRequestHandler<CreateWebhookCommand, Result<WebhookDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public CreateWebhookCommandHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<WebhookDto>> Handle(CreateWebhookCommand request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<WebhookDto>.Failure("Company context is required");

        var webhook = new Webhook(
            companyId.Value,
            request.Name.Trim(),
            request.Url.Trim(),
            request.Events?.Trim() ?? "",
            request.Secret?.Trim(),
            request.Description?.Trim(),
            _currentUser.UserId);
        _context.Webhooks.Add(webhook);
        await _context.SaveChangesAsync(cancellationToken);

        return Result<WebhookDto>.Success(Map(webhook));
    }

    private static WebhookDto Map(Webhook w) => new()
    {
        Id = w.Id,
        Name = w.Name,
        Url = w.Url,
        Events = w.Events,
        IsActive = w.IsActive,
        Description = w.Description,
        CreatedAt = w.CreatedAt
    };
}
