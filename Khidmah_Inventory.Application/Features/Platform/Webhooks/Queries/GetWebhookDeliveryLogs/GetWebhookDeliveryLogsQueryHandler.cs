using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Queries.GetWebhookDeliveryLogs;

public class GetWebhookDeliveryLogsQueryHandler : IRequestHandler<GetWebhookDeliveryLogsQuery, Result<PagedResult<WebhookDeliveryLogDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWebhookDeliveryLogsQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<PagedResult<WebhookDeliveryLogDto>>> Handle(GetWebhookDeliveryLogsQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<PagedResult<WebhookDeliveryLogDto>>.Failure("Company context is required");

        var webhook = await _context.Webhooks
            .FirstOrDefaultAsync(w => w.Id == request.WebhookId && w.CompanyId == companyId.Value && !w.IsDeleted, cancellationToken);
        if (webhook == null)
            return Result<PagedResult<WebhookDeliveryLogDto>>.Failure("Webhook not found");

        var query = _context.WebhookDeliveryLogs
            .Where(l => l.WebhookId == request.WebhookId && l.CompanyId == companyId.Value)
            .OrderByDescending(l => l.DeliveredAt);

        var total = await query.CountAsync(cancellationToken);
        var items = await query
            .Skip((request.PageNo - 1) * request.PageSize)
            .Take(request.PageSize)
            .Select(l => new WebhookDeliveryLogDto
            {
                Id = l.Id,
                EventName = l.EventName,
                HttpStatusCode = l.HttpStatusCode,
                Success = l.Success,
                RetryCount = l.RetryCount,
                ErrorMessage = l.ErrorMessage,
                DeliveredAt = l.DeliveredAt
            })
            .ToListAsync(cancellationToken);

        return Result<PagedResult<WebhookDeliveryLogDto>>.Success(new PagedResult<WebhookDeliveryLogDto>
        {
            Items = items,
            TotalCount = total,
            PageNo = request.PageNo,
            PageSize = request.PageSize
        });
    }
}
