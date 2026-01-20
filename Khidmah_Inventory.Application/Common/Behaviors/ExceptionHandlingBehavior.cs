using MediatR;
using Microsoft.Extensions.Logging;
using Khidmah_Inventory.Application.Common.Models;

namespace Khidmah_Inventory.Application.Common.Behaviors;

public class ExceptionHandlingBehavior<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
    where TRequest : IRequest<TResponse>
{
    private readonly ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> _logger;

    public ExceptionHandlingBehavior(ILogger<ExceptionHandlingBehavior<TRequest, TResponse>> logger)
    {
        _logger = logger;
    }

    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        try
        {
            return await next();
        }
        catch (ArgumentException ex)
        {
            _logger.LogWarning(ex, "Validation error in {RequestName}", typeof(TRequest).Name);
            
            // Handle Result<T> types
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var dataType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result<>).MakeGenericType(dataType).GetMethod("Failure", new[] { typeof(string[]) });
                var result = failureMethod?.Invoke(null, new object[] { new[] { ex.Message } });
                return (TResponse)(result ?? throw new InvalidOperationException("Failed to create failure result"));
            }
            
            // Handle Result type
            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Failure(ex.Message);
            }
            
            throw;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception in {RequestName}", typeof(TRequest).Name);
            
            // Handle Result<T> types
            if (typeof(TResponse).IsGenericType && typeof(TResponse).GetGenericTypeDefinition() == typeof(Result<>))
            {
                var dataType = typeof(TResponse).GetGenericArguments()[0];
                var failureMethod = typeof(Result<>).MakeGenericType(dataType).GetMethod("Failure", new[] { typeof(string[]) });
                var result = failureMethod?.Invoke(null, new object[] { new[] { "An error occurred while processing your request." } });
                return (TResponse)(result ?? throw new InvalidOperationException("Failed to create failure result"));
            }
            
            // Handle Result type
            if (typeof(TResponse) == typeof(Result))
            {
                return (TResponse)(object)Result.Failure("An error occurred while processing your request.");
            }
            
            throw;
        }
    }
}

