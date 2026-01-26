using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Khidmah_Inventory.API.Attributes;

/// <summary>
/// Validates API code from request header or body
/// Usage: [ValidateApiCode(ProductsModuleCode.ViewAll)]
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = false)]
public class ValidateApiCodeAttribute : ActionFilterAttribute
{
    private readonly string _expectedApiCode;

    public ValidateApiCodeAttribute(string expectedApiCode)
    {
        _expectedApiCode = expectedApiCode;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        // Try to get API code from header first
        var apiCode = context.HttpContext.Request.Headers["X-Api-Code"].FirstOrDefault();

        // If not in header, try to get from query string
        if (string.IsNullOrEmpty(apiCode))
        {
            apiCode = context.HttpContext.Request.Query["apiCode"].FirstOrDefault();
        }

        // If still not found, try to get from request body (check all action arguments)
        if (string.IsNullOrEmpty(apiCode) && context.ActionArguments.Count > 0)
        {
            foreach (var argument in context.ActionArguments.Values)
            {
                if (argument != null)
                {
                    var apiCodeProperty = argument.GetType().GetProperty("ApiCode");
                    if (apiCodeProperty != null)
                    {
                        apiCode = apiCodeProperty.GetValue(argument)?.ToString();
                        if (!string.IsNullOrEmpty(apiCode))
                        {
                            break;
                        }
                    }
                }
            }
        }

        // Validate API code only if provided (optional validation)
        // If no API code is provided, allow the request to proceed
        if (!string.IsNullOrEmpty(apiCode) && apiCode != _expectedApiCode)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<ValidateApiCodeAttribute>>();
            logger?.LogWarning("API code validation failed. Expected: {Expected}, Received: {Received}, Path: {Path}",
                _expectedApiCode, apiCode, context.HttpContext.Request.Path);

            context.Result = new BadRequestObjectResult(new
            {
                success = false,
                message = "Invalid API code",
                statusCode = 400,
                errors = new[] { $"Expected API code: {_expectedApiCode}, but received: {apiCode}" }
            });
            return;
        }

        base.OnActionExecuting(context);
    }
}
