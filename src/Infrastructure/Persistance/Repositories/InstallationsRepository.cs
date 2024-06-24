using Application.Common.Repositories;
using Domain.Installations;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories;

public sealed class InstallationsRepository : IInstallationsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public InstallationsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Installation> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Installations.FindAsync(id, cancellationToken);
    }

    public async Task<List<Installation>> GetInstallationsByCodesAsync(List<string> codes, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Installations.Where(i => codes.Contains(i.Code)).ToListAsync();
    }
}
