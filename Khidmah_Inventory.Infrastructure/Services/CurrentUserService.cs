using System.Security.Claims;
using Khidmah_Inventory.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Khidmah_Inventory.Infrastructure.Services;

public class CurrentUserService : ICurrentUserService
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public Guid? UserId
    {
        get
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier);
            return userIdClaim != null && Guid.TryParse(userIdClaim.Value, out var userId) ? userId : null;
        }
    }

    public Guid? CompanyId
    {
        get
        {
            var companyIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("CompanyId");
            return companyIdClaim != null && Guid.TryParse(companyIdClaim.Value, out var companyId) ? companyId : null;
        }
    }

    public string? Email => _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Email)?.Value;

    public bool IsAuthenticated => _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;

    public bool HasPermission(string permission)
    {
        return _httpContextAccessor.HttpContext?.User?.Claims
            .Any(c => c.Type == "Permission" && c.Value == permission) ?? false;
    }

    public bool HasRole(string role)
    {
        return _httpContextAccessor.HttpContext?.User?.IsInRole(role) ?? false;
    }
}

