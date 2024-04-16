using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace ChemDec.Api;

public static class CommandAndQueryHandlersSetup
{
    public static void AddCommandOrQueryHandlers(IServiceCollection services, Type interfaceType)
    {
        var types = interfaceType.Assembly.GetTypes().Where(t =>
            t.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType));
        foreach (var type in types)
        {
            type.GetInterfaces().Where(i => i.IsGenericType && i.GetGenericTypeDefinition() == interfaceType)
                .ToList().ForEach(i => services.AddScoped(i, type));
        }
    }
}
