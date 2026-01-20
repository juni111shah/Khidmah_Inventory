using System.Net;
using System.Text.Json;
using FluentValidation;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.API.Middleware;

public class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An unhandled exception occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var code = HttpStatusCode.InternalServerError;
        var result = string.Empty;

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                result = JsonSerializer.Serialize(new Result
                {
                    Succeeded = false,
                    Errors = validationException.Errors.Select(e => e.ErrorMessage).ToArray()
                });
                break;

            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                result = JsonSerializer.Serialize(new Result
                {
                    Succeeded = false,
                    Errors = new[] { "Unauthorized access" }
                });
                break;

            default:
                result = JsonSerializer.Serialize(new Result
                {
                    Succeeded = false,
                    Errors = new[] { "An error occurred while processing your request" }
                });
                break;
        }

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}

