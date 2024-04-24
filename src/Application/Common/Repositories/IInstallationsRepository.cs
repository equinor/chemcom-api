using Domain.Installations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Repositories;

public interface IInstallationsRepository
{
    Task<Installation> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
}
