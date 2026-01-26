using System.Net;
using System.Text.Json;
using FluentValidation;
using Khidmah_Inventory.API.Models;
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
        var message = "An error occurred while processing your request";
        var errors = new List<string>();

        switch (exception)
        {
            case ValidationException validationException:
                code = HttpStatusCode.BadRequest;
                message = "Validation failed";
                errors.AddRange(validationException.Errors.Select(e => e.ErrorMessage));
                break;

            case UnauthorizedAccessException:
                code = HttpStatusCode.Unauthorized;
                message = "Unauthorized access";
                errors.Add("You do not have permission to perform this action.");
                break;

            case NullReferenceException nullRefEx:
                code = HttpStatusCode.BadRequest;
                message = "A required value was not found";
                errors.Add($"Null reference: {nullRefEx.Message}");
                if (nullRefEx.StackTrace != null)
                {
                    var stackTraceLines = nullRefEx.StackTrace.Split('\n').Take(3);
                    errors.Add($"Location: {string.Join(" -> ", stackTraceLines)}");
                }
                break;

            case ArgumentNullException argNullEx:
                code = HttpStatusCode.BadRequest;
                message = "A required parameter was not provided";
                errors.Add($"Missing parameter: {argNullEx.ParamName ?? argNullEx.Message}");
                break;

            default:
                code = HttpStatusCode.BadRequest;
                errors.Add(exception.Message);
                if (exception.InnerException != null)
                {
                    errors.Add($"Inner exception: {exception.InnerException.Message}");
                }
                // Include stack trace in development
                var env = context.RequestServices.GetService<Microsoft.AspNetCore.Hosting.IWebHostEnvironment>();
                if (env?.IsDevelopment() == true && exception.StackTrace != null)
                {
                    var stackTraceLines = exception.StackTrace.Split('\n').Take(5);
                    errors.Add($"Stack trace: {string.Join(" -> ", stackTraceLines)}");
                }
                break;
        }

        var response = ApiResponse<object>.FailureResponse(message, (int)code, errors.ToArray());

        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        var result = JsonSerializer.Serialize(response, options);

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)code;
        return context.Response.WriteAsync(result);
    }
}

