namespace MicroERP.Application.Common.Models;

public class Result
{
    public bool Success { get; set; }

    public string Message { get; set; } = string.Empty;

    public static Result Succeeded(string message = "")
    {
        return new Result
        {
            Success = true,
            Message = message
        };
    }

    public static Result Failure(string message)
    {
        return new Result
        {
            Success = false,
            Message = message
        };
    }

}