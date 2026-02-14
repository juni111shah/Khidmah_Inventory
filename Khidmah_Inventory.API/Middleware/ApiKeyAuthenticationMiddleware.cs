using System.Security.Claims;
using Khidmah_Inventory.Application.Common.Interfaces;

namespace Khidmah_Inventory.API.Middleware;

/// <summary>
/// When request has no JWT but has X-Api-Key (or Api-Key) header, validate key and set User with CompanyId + Permission claims.
/// </summary>
public class ApiKeyAuthenticationMiddleware
{
    public const string ApiKeyHeaderName = "X-Api-Key";
    private const string ApiKeyIdClaim = "ApiKeyId";

    private readonly RequestDelegate _next;

    public ApiKeyAuthenticationMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, IApiKeyService apiKeyService)
    {
        if (context.User.Identity?.IsAuthenticated == true)
        {
            await _next(context);
            return;
        }

        var rawKey = context.Request.Headers[ApiKeyHeaderName].FirstOrDefault()
            ?? context.Request.Headers["Api-Key"].FirstOrDefault();
        if (string.IsNullOrWhiteSpace(rawKey))
        {
            await _next(context);
            return;
        }

        var apiKey = await apiKeyService.ValidateKeyAsync(rawKey, context.RequestAborted);
        if (apiKey == null)
        {
            await _next(context);
            return;
        }

        var claims = new List<Claim>
        {
            new(ClaimTypes.NameIdentifier, apiKey.Id.ToString()),
            new("CompanyId", apiKey.CompanyId.ToString()),
            new(ApiKeyIdClaim, apiKey.Id.ToString())
        };
        if (!string.IsNullOrWhiteSpace(apiKey.Permissions))
        {
            foreach (var p in apiKey.Permissions.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
                claims.Add(new Claim("Permission", p.Trim()));
        }
        var identity = new ClaimsIdentity(claims, "ApiKey");
        context.User = new ClaimsPrincipal(identity);
        await _next(context);
    }
}
