using Khidmah_Inventory.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Khidmah_Inventory.Infrastructure.Services;

public class MultiTenantService : IMultiTenantService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ICurrentUserService _currentUserService;

    public MultiTenantService(
        IHttpContextAccessor httpContextAccessor,
        ICurrentUserService currentUserService)
    {
        _httpContextAccessor = httpContextAccessor;
        _currentUserService = currentUserService;
    }

    public bool IsMultiTenantEnabled => true;

    public Guid? GetCurrentCompanyId()
    {
        // First try to get from HTTP header (for API calls)
        var headerCompanyId = _httpContextAccessor.HttpContext?.Request.Headers["X-Company-Id"].FirstOrDefault();
        if (!string.IsNullOrEmpty(headerCompanyId) && Guid.TryParse(headerCompanyId, out var headerGuid))
        {
            return headerGuid;
        }

        // Fall back to user's company from JWT token
        return _currentUserService.CompanyId;
    }

    public void SetCurrentCompanyId(Guid companyId)
    {
        // Store in HTTP context items for the current request
        _httpContextAccessor.HttpContext?.Items.TryAdd("CurrentCompanyId", companyId);
    }
}

