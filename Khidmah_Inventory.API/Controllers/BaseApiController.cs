using MediatR;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.API.Models;

namespace Khidmah_Inventory.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseApiController : ControllerBase
{
    private IMediator? _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();

    /// <summary>
    /// Handles the result from a MediatR command/query and returns standardized API response
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result, string? successMessage = null)
    {
        if (result.Succeeded && result.Data != null)
        {
            var response = ApiResponse<T>.SuccessResponse(
                result.Data,
                successMessage ?? "Operation completed successfully",
                200
            );
            return Ok(response);
        }

        if (result.Succeeded && result.Data == null)
        {
            var response = ApiResponse<T>.FailureResponse(
                "Resource not found",
                404,
                "The requested resource was not found"
            );
            return NotFound(response);
        }

        var errorResponse = ApiResponse<T>.FailureResponse(
            "Operation failed",
            result.Errors,
            400
        );
        return BadRequest(errorResponse);
    }

    /// <summary>
    /// Handles the result from a MediatR command/query and returns standardized API response with custom success status
    /// </summary>
    protected IActionResult HandleResult<T>(Result<T> result, int successStatusCode, string? successMessage = null)
    {
        if (result.Succeeded && result.Data != null)
        {
            var response = ApiResponse<T>.SuccessResponse(
                result.Data,
                successMessage ?? "Operation completed successfully",
                successStatusCode
            );
            return StatusCode(successStatusCode, response);
        }

        if (result.Succeeded && result.Data == null)
        {
            var response = ApiResponse<T>.FailureResponse(
                "Resource not found",
                404,
                "The requested resource was not found"
            );
            return NotFound(response);
        }

        var errorResponse = ApiResponse<T>.FailureResponse(
            "Operation failed",
            result.Errors,
            400
        );
        return BadRequest(errorResponse);
    }

    /// <summary>
    /// Handles the result from a MediatR command/query that returns void
    /// </summary>
    protected IActionResult HandleResult(Result result, string? successMessage = null)
    {
        if (result.Succeeded)
        {
            var response = ApiResponse.SuccessResponse(
                successMessage ?? "Operation completed successfully",
                200
            );
            return Ok(response);
        }

        var errorResponse = ApiResponse.FailureResponse(
            "Operation failed",
            400,
            result.Errors.ToArray()
        );
        return BadRequest(errorResponse);
    }

    /// <summary>
    /// Returns a standardized success response with data
    /// </summary>
    protected IActionResult SuccessResponse<T>(T data, string message = "Operation completed successfully", int statusCode = 200)
    {
        var response = ApiResponse<T>.SuccessResponse(data, message, statusCode);
        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Returns a standardized success response without data
    /// </summary>
    protected IActionResult SuccessResponse(string message = "Operation completed successfully", int statusCode = 200)
    {
        var response = ApiResponse.SuccessResponse(message, statusCode);
        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Returns a standardized failure response
    /// </summary>
    protected IActionResult FailureResponse(string message, int statusCode = 400, params string[] errors)
    {
        var response = ApiResponse.FailureResponse(message, statusCode, errors);
        return StatusCode(statusCode, response);
    }

    /// <summary>
    /// Returns a standardized not found response
    /// </summary>
    protected IActionResult NotFoundResponse(string message = "Resource not found")
    {
        var response = ApiResponse<object>.FailureResponse(
            message,
            404,
            "The requested resource was not found"
        );
        return NotFound(response);
    }
}
