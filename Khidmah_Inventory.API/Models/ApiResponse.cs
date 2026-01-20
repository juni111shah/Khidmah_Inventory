namespace Khidmah_Inventory.API.Models;

/// <summary>
/// Standardized API response structure for all endpoints
/// </summary>
public class ApiResponse<T>
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// Response data (null if operation failed)
    /// </summary>
    public T? Data { get; set; }

    /// <summary>
    /// List of errors (empty if operation succeeded)
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response with data
    /// </summary>
    public static ApiResponse<T> SuccessResponse(T data, string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            StatusCode = statusCode,
            Data = data,
            Errors = new List<string>(),
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a successful response without data
    /// </summary>
    public static ApiResponse<T> SuccessResponse(string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponse<T>
        {
            Success = true,
            Message = message,
            StatusCode = statusCode,
            Data = default,
            Errors = new List<string>(),
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a failure response with errors
    /// </summary>
    public static ApiResponse<T> FailureResponse(string message, int statusCode = 400, params string[] errors)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Data = default,
            Errors = errors.ToList(),
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a failure response with errors from a list
    /// </summary>
    public static ApiResponse<T> FailureResponse(string message, IEnumerable<string> errors, int statusCode = 400)
    {
        return new ApiResponse<T>
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Data = default,
            Errors = errors.ToList(),
            Timestamp = DateTime.UtcNow
        };
    }
}

/// <summary>
/// Non-generic API response for operations that don't return data
/// </summary>
public class ApiResponse
{
    /// <summary>
    /// Indicates whether the operation was successful
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// HTTP status code
    /// </summary>
    public int StatusCode { get; set; }

    /// <summary>
    /// List of errors (empty if operation succeeded)
    /// </summary>
    public List<string> Errors { get; set; } = new();

    /// <summary>
    /// Timestamp of the response
    /// </summary>
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;

    /// <summary>
    /// Creates a successful response
    /// </summary>
    public static ApiResponse SuccessResponse(string message = "Operation completed successfully", int statusCode = 200)
    {
        return new ApiResponse
        {
            Success = true,
            Message = message,
            StatusCode = statusCode,
            Errors = new List<string>(),
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a failure response with errors
    /// </summary>
    public static ApiResponse FailureResponse(string message, int statusCode = 400, params string[] errors)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors.ToList(),
            Timestamp = DateTime.UtcNow
        };
    }

    /// <summary>
    /// Creates a failure response with errors from a list
    /// </summary>
    public static ApiResponse FailureResponse(string message, IEnumerable<string> errors, int statusCode = 400)
    {
        return new ApiResponse
        {
            Success = false,
            Message = message,
            StatusCode = statusCode,
            Errors = errors.ToList(),
            Timestamp = DateTime.UtcNow
        };
    }
}

