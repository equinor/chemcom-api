using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace Application.Common;

public interface ICommandDispatcher
{
    Task DispatchAsync<T>(T command) where T : ICommand;
    Task<TResult> DispatchAsync<TCommand, TResult>(TCommand command) where TCommand : ICommand;
}
