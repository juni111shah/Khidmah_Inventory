using System.Reflection;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Khidmah_Inventory.Application.Common.Models;
using Khidmah_Inventory.API.Models;

namespace Khidmah_Inventory.API.Controllers;

/// <summary>
/// Base controller with standardized request execution methods
/// </summary>
public abstract class BaseController : ControllerBase
{
    protected readonly IMediator Mediator;

    protected BaseController(IMediator mediator)
    {
        Mediator = mediator;
    }

    /// <summary>
    /// Executes a MediatR request and returns standardized API response
    /// </summary>
    protected async Task<IActionResult> ExecuteRequest<TRequest, TResponse>(TRequest request)
        where TRequest : IRequest<Result<TResponse>>
    {
        try
        {
            var result = await Mediator.Send(request);
            return HandleResult(result);
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            var logger = HttpContext.RequestServices.GetService<ILogger<BaseController>>();
            logger?.LogError(ex, "Exception in ExecuteRequest for {RequestType}", typeof(TRequest).Name);
            throw; // Re-throw to be caught by ExceptionHandlingMiddleware
        }
    }

    /// <summary>
    /// Executes a MediatR request with automatic type inference
    /// This method automatically determines the response type from the request and calls the appropriate overload
    /// </summary>
    protected Task<IActionResult> ExecuteRequest<TRequest>(TRequest request)
    {
        try
        {
            // Extract the response type from IRequest<Result<TResponse>> or IRequest<Result>
            var requestType = typeof(TRequest);
            var requestInterface = requestType.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

            if (requestInterface == null)
            {
                throw new InvalidOperationException($"Request type {requestType.Name} does not implement IRequest<>");
            }

            var responseType = requestInterface.GetGenericArguments()[0];
            
            // Check if it's Result<TResponse>
            if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
            {
                var dataType = responseType.GetGenericArguments()[0];
                // Use reflection to call ExecuteRequest<TRequest, TResponse>
                var method = typeof(BaseController).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(m => m.Name == nameof(ExecuteRequest) && m.GetGenericArguments().Length == 2);
                
                var genericMethod = method.MakeGenericMethod(typeof(TRequest), dataType);
                return (Task<IActionResult>)genericMethod.Invoke(this, new object[] { request })!;
            }
            else if (responseType == typeof(Result))
            {
                // For IRequest<Result>, use reflection to call ExecuteRequestResult
                var method = typeof(BaseController).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                    .First(m => m.Name == nameof(ExecuteRequestResult) && m.GetGenericArguments().Length == 1);
                
                var genericMethod = method.MakeGenericMethod(typeof(TRequest));
                return (Task<IActionResult>)genericMethod.Invoke(this, new object[] { request })!;
            }

            throw new InvalidOperationException($"Unsupported response type: {responseType.Name}");
        }
        catch (Exception ex)
        {
            // Log the exception for debugging
            var logger = HttpContext.RequestServices.GetService<ILogger<BaseController>>();
            logger?.LogError(ex, "Exception in ExecuteRequest for {RequestType}", typeof(TRequest).Name);
            throw; // Re-throw to be caught by ExceptionHandlingMiddleware
        }
    }

    /// <summary>
    /// Helper method to execute requests that return Result (not Result<T>)
    /// </summary>
    private async Task<IActionResult> ExecuteRequestResult<TRequest>(TRequest request)
        where TRequest : IRequest<Result>
    {
        var result = await Mediator.Send(request);
        return HandleResult(result);
    }

    /// <summary>
    /// Executes a MediatR request with caching support
    /// For now, this just calls ExecuteRequest since caching is not implemented
    /// </summary>
    protected Task<IActionResult> ExecuteRequestWithCache<TRequest>(TRequest request)
    {
        // Extract the response type from IRequest<Result<TResponse>> or IRequest<Result>
        var requestType = typeof(TRequest);
        var requestInterface = requestType.GetInterfaces()
            .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IRequest<>));

        if (requestInterface == null)
        {
            throw new InvalidOperationException($"Request type {requestType.Name} does not implement IRequest<>");
        }

        var responseType = requestInterface.GetGenericArguments()[0];
        
        // Check if it's Result<TResponse>
        if (responseType.IsGenericType && responseType.GetGenericTypeDefinition() == typeof(Result<>))
        {
            var dataType = responseType.GetGenericArguments()[0];
            // Use reflection to call ExecuteRequest<TRequest, TResponse>
            var method = typeof(BaseController).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == nameof(ExecuteRequest) && m.GetGenericArguments().Length == 2);
            
            var genericMethod = method.MakeGenericMethod(typeof(TRequest), dataType);
            return (Task<IActionResult>)genericMethod.Invoke(this, new object[] { request })!;
        }
        else if (responseType == typeof(Result))
        {
            // Use the non-generic ExecuteRequest
            var method = typeof(BaseController).GetMethods(BindingFlags.NonPublic | BindingFlags.Instance)
                .First(m => m.Name == nameof(ExecuteRequest) && m.GetGenericArguments().Length == 1);
            
            var genericMethod = method.MakeGenericMethod(typeof(TRequest));
            return (Task<IActionResult>)genericMethod.Invoke(this, new object[] { request })!;
        }

        throw new InvalidOperationException($"Unsupported response type: {responseType.Name}");
    }

    /// <summary>
    /// Handles the result from a MediatR command/query and returns standardized API response
    /// </summary>
    private IActionResult HandleResult<T>(Result<T> result)
    {
        if (result.Succeeded && result.Data != null)
        {
            var response = ApiResponse<T>.SuccessResponse(
                result.Data,
                "Operation completed successfully",
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
    /// Handles the result from a MediatR command/query that returns void
    /// </summary>
    private IActionResult HandleResult(Result result)
    {
        if (result.Succeeded)
        {
            var response = ApiResponse.SuccessResponse(
                "Operation completed successfully",
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
}
