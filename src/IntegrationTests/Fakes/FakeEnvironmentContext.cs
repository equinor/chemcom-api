using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Fakes;

public class FakeEnvironmentContext : IEnvironmentContext
{
    public string GetEnvironmentPrefix()
    {
        return "[Test] ";
    }

    public string GetFromEmailAddress()
    {
        return "test@equinot.com";
    }

    public string GetHostedEnvironment()
    {
        return "Dev";
    }

    public string GetPortalUrl()
    {
        return "https://localhost";
    }
}
