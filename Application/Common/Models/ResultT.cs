namespace MicroERP.Application.Common.Models;

public class Result<T>
{
    public bool Success { get; set; }
    
    public string Message { get; set; } = string.Empty;

    public T? Data { get; set; }

    public static Result<T> Succeeded(
        T data,
        string message = "")
    {
        return new Result<T>
        {
            Success = true,
            Message = message,
            Data = data
        };
    }

    public static Result<T> Failure(
        string message)
    {
        return new Result<T>
        {
            Success = false,
            Message = message
        };
    }
    public Result ToResult()
    {
        return Success
            ? Result.Succeeded(Message)
            : Result.Failure(Message);
    }
}