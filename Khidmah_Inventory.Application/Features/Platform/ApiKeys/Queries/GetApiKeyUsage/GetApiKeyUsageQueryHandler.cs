using MediatR;
using Microsoft.EntityFrameworkCore;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.Application.Features.Platform.ApiKeys.Models;

namespace Khidmah_Inventory.Application.Features.Platform.ApiKeys.Queries.GetApiKeyUsage;

public class GetApiKeyUsageQueryHandler : IRequestHandler<GetApiKeyUsageQuery, Result<ApiKeyUsageDto>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUserService _currentUser;

    public GetApiKeyUsageQueryHandler(IApplicationDbContext context, ICurrentUserService currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<Result<ApiKeyUsageDto>> Handle(GetApiKeyUsageQuery request, CancellationToken cancellationToken)
    {
        var companyId = _currentUser.CompanyId;
        if (!companyId.HasValue)
            return Result<ApiKeyUsageDto>.Failure("Company context is required");

        IQueryable<Domain.Entities.ApiKey> keysQuery = _context.ApiKeys
            .Where(k => k.CompanyId == companyId.Value && !k.IsDeleted);
        if (request.ApiKeyId.HasValue)
            keysQuery = keysQuery.Where(k => k.Id == request.ApiKeyId.Value);

        var keys = await keysQuery.ToListAsync(cancellationToken);
        if (keys.Count == 0)
            return Result<ApiKeyUsageDto>.Failure("No API keys found");

        var key = keys.First();
        var usage = new ApiKeyUsageDto
        {
            ApiKeyId = key.Id,
            Name = request.ApiKeyId.HasValue ? key.Name : "All keys",
            TotalCalls = keys.Sum(k => k.RequestCount),
            ErrorCalls = keys.Sum(k => k.ErrorCount),
            LastAccessAt = keys.Max(k => k.LastUsedAt)
        };

        var keyIds = keys.Select(k => k.Id).ToList();
        var logs = await _context.ApiKeyUsageLogs
            .Where(l => keyIds.Contains(l.ApiKeyId))
            .OrderByDescending(l => l.CreatedAt)
            .Take(request.RecentLogsCount)
            .Select(l => new ApiKeyUsageLogDto
            {
                Method = l.Method,
                Path = l.Path,
                StatusCode = l.StatusCode,
                Success = l.Success,
                ElapsedMs = l.ElapsedMs,
                CreatedAt = l.CreatedAt
            })
            .ToListAsync(cancellationToken);
        usage.RecentLogs = logs;

        return Result<ApiKeyUsageDto>.Success(usage);
    }
}
