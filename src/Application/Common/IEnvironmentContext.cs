using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public interface IEnvironmentContext
{
    string GetEnvironmentPrefix();
    string GetFromEmailAddress();
    string GetHostedEnvironment();
    string GetPortalUrl();
}
