using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public sealed record Result
{
    public Result()
    {
        Errors = new List<string>();
    }
    public List<string> Errors { get; set; }
    public object Data { get; set; }
}
