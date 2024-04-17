using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public sealed record Result<T>
{
    public Result()
    {
        Errors = new List<string>();
    }
    public List<string> Errors { get; set; }
    public T Data { get; set; }
    public string Status { get; set; }

}

public static class ResultStatusConstants
{
    public static readonly string Success = "Success";
    public static readonly string Failed = "Failed";
    public static readonly string NotFound = "NotFound";
    public static readonly string NotAllowed = "NotAllowed";
}
