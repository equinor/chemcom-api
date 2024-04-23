using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public interface IQueryHandler<TQuery, TResult> where TQuery : IQuery
{
    Task<TResult> ExecuteAsync(TQuery query, CancellationToken cancellationToken = default);
}
