using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public sealed record Result<T>
{
    public Result() { }
    public List<string> Errors { get; private set; }
    public T Data { get; set; }
    public string Status { get; set; }

    public static Result<T> Success(T data)
    {
        return new Result<T>
        {
            Data = data,
            Status = ResultStatusConstants.Success
        };
    }

    public static Result<T> Failed(List<string> errors)
    {
        return new Result<T>
        {
            Errors = errors,
            Status = ResultStatusConstants.Failed
        };
    }

    public static Result<T> NotFound(List<string> errors)
    {
        return new Result<T>
        {
            Errors = errors,
            Status = ResultStatusConstants.NotFound
        };
    }
}

public sealed record Result
{
    public Result()
    { }
    public List<string> Errors { get; set; }
    public string Status { get; set; }
}

public static class ResultStatusConstants
{
    public static readonly string Success = "Success";
    public static readonly string Failed = "Failed";
    public static readonly string NotFound = "NotFound";
    public static readonly string NotAllowed = "NotAllowed";
}
