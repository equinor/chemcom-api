using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public sealed class Result<T> : ResultBase
{
    public Result() { }
    public T Data { get; private set; }

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

public abstract class ResultBase
{
    public ResultBase()
    { }
    public List<string> Errors { get; protected set; }
    public string Status { get; protected set; }
}

public static class ResultStatusConstants
{
    public static readonly string Success = "Success";
    public static readonly string Failed = "Failed";
    public static readonly string NotFound = "NotFound";
    public static readonly string NotAllowed = "NotAllowed";
}
