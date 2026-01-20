using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.API.Middleware;

public class MultiTenantMiddleware
{
    private readonly RequestDelegate _next;

    public MultiTenantMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IMultiTenantService multiTenantService)
    {
        // Extract company ID from header or token
        var companyIdHeader = context.Request.Headers["X-Company-Id"].FirstOrDefault();
        
        if (!string.IsNullOrEmpty(companyIdHeader) && Guid.TryParse(companyIdHeader, out var companyId))
        {
            multiTenantService.SetCurrentCompanyId(companyId);
        }

        await _next(context);
    }
}

