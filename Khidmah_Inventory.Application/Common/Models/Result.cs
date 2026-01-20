namespace Khidmah_Inventory.Application.Common.Models;

public class Result
{
    public bool Succeeded { get; set; }
    public string[] Errors { get; set; } = Array.Empty<string>();

    public static Result Success() => new() { Succeeded = true };
    public static Result Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
}

public class Result<T> : Result
{
    public T? Data { get; set; }

    public static Result<T> Success(T data) => new() { Succeeded = true, Data = data };
    public static new Result<T> Failure(params string[] errors) => new() { Succeeded = false, Errors = errors };
}

