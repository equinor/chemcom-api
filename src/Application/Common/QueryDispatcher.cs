using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public sealed class QueryDispatcher : IQueryDispatcher
{
    private readonly IServiceProvider _serviceProvider;

    public QueryDispatcher(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }
    public async Task<TResult> DispatchAsync<TQuery, TResult>(TQuery query) where TQuery : IQuery
    {
        return _serviceProvider.GetService(typeof(IQueryHandler<TQuery, TResult>)) is not IQueryHandler<TQuery, TResult> handler
            ? throw new ApplicationException($"No Queryhandler registered for handling {typeof(TQuery)}")
            : await handler.ExecuteAsync(query);
    }
}
