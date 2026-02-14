using System.Diagnostics;
using Khidmah_Inventory.Application.Common.Interfaces;
using Khidmah_Inventory.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Khidmah_Inventory.API.Middleware;

/// <summary>
/// After the pipeline runs, if the request was authenticated via API key, update key usage and optionally log.
/// </summary>
public class ApiKeyUsageMiddleware
{
    private readonly RequestDelegate _next;
    private const string ApiKeyIdClaim = "ApiKeyId";
    private const string StartTimeKey = "ApiKeyUsageStartMs";

    public ApiKeyUsageMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var stopwatch = Stopwatch.StartNew();
        if (context.User?.FindFirst(ApiKeyIdClaim)?.Value is { } keyIdStr && Guid.TryParse(keyIdStr, out var apiKeyId))
            context.Items[StartTimeKey] = stopwatch;
        await _next(context);
        stopwatch.Stop();
        if (context.User?.FindFirst(ApiKeyIdClaim)?.Value is not { } idStr || !Guid.TryParse(idStr, out var keyId))
            return;
        var statusCode = context.Response.StatusCode;
        var success = statusCode >= 200 && statusCode < 300;
        var path = context.Request.Path.Value ?? "";
        var method = context.Request.Method;
        var elapsedMs = stopwatch.ElapsedMilliseconds;
        _ = Task.Run(async () =>
        {
            try
            {
                using var scope = context.RequestServices.CreateScope();
                var db = scope.ServiceProvider.GetRequiredService<IApplicationDbContext>();
                var key = await db.ApiKeys.FirstOrDefaultAsync(k => k.Id == keyId);
                if (key == null) return;
                key.UpdateUsage(success);
                db.ApiKeyUsageLogs.Add(new ApiKeyUsageLog(key.CompanyId, key.Id, method, path, statusCode, success, elapsedMs));
                await db.SaveChangesAsync(CancellationToken.None);
            }
            catch
            {
                // Don't fail the request if logging fails
            }
        });
    }
}
