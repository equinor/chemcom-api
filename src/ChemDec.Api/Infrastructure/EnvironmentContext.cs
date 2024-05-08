using Application.Common;
using Microsoft.Extensions.Configuration;

namespace ChemDec.Api.Infrastructure;

public class EnvironmentContext : IEnvironmentContext
{
    private readonly IConfiguration _configuration;

    public EnvironmentContext(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public string GetChemicalEmail()
    {
        return GetEnvironmentVariable("chemicalEmail");
    }

    public string GetPortalUrl()
    {
        if (_configuration["env"] == "Dev")
        {
            return "https://frontend-chemcom-dev.radix.equinor.com";
        }

        return "https://chemcom.equinor.com";
    }

    public string GetHostedEnvironment()
    {
        return GetEnvironmentVariable("env");
    }

    public string GetEnvironmentPrefix()
    {
        switch (_configuration["env"])
        {
            case "Dev":
                return "[DEV] ";
            case "Local":
                return "[Local] ";
            default:
                break;
        }

        return "";
    }

    public string GetFromEmailAddress()
    {
        return GetEnvironmentVariable("FromEmailAddress");
    }

    private string GetEnvironmentVariable(string key)
    {
        return _configuration[key];
    }
}
