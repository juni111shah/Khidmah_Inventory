using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.Webhooks.Models;

namespace Khidmah_Inventory.Application.Features.Platform.Webhooks.Queries.GetWebhooksList;

public class GetWebhooksListQueryHandler : IRequestHandler<GetWebhooksListQuery, Result<List<WebhookDto>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetWebhooksListQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<List<WebhookDto>>> Handle(GetWebhooksListQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<List<WebhookDto>>.Failure("Company context is required");

        var query = _context.Webhooks
            .Where(w => w.CompanyId == companyId.Value && !w.IsDeleted)
            .AsQueryable();
        if (request.IsActive.HasValue)
            query = query.Where(w => w.IsActive == request.IsActive.Value);

        var list = await query
            .OrderBy(w => w.Name)
            .Select(w => new WebhookDto
            {
                Id = w.Id,
                Name = w.Name,
                Url = w.Url,
                Events = w.Events,
                IsActive = w.IsActive,
                Description = w.Description,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync(cancellationToken);
        return Result<List<WebhookDto>>.Success(list);
    }
}
