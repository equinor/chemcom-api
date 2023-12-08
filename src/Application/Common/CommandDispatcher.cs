using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;
public sealed class CommandDispatcher : ICommandDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public CommandDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task DispatchAsync<T>(T command) where T : ICommand
    {
        if (_serviceProvider.GetService(typeof(ICommandHandler<T>)) is not ICommandHandler<T> handler)
        {
            throw new ApplicationException($"No Commandhandler registered for handling {typeof(T)}");
        }
        await handler.HandleAsync(command);
    }

    public async Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand
    {
        return _serviceProvider.GetService(typeof(ICommandHandler<TCommand, TResult>)) is not ICommandHandler<TCommand, TResult> handler
            ? throw new ApplicationException($"No Commandhandler registered for handling {typeof(TCommand)}")
            : await handler.HandleAsync(command);
    }
}

