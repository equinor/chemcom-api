using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services;

public sealed record EmailResponse
{
    public bool IsSuccessful { get; private set; }
    public string Error { get; private set; }

    public static EmailResponse Success() => new EmailResponse { IsSuccessful = true };
    public static EmailResponse Fail(string error) => new EmailResponse { IsSuccessful = false, Error = error };
}
