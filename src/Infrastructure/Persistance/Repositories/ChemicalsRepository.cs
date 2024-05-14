using Application.Common.Repositories;
using Domain.Chemicals;
using Domain.ShipmentChemicals;
using Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories;

public sealed class ChemicalsRepository : IChemicalsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ChemicalsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    //TODO: Added the same logic from the legacy code and seems like the implementation is messey. Come back to it.
    public async Task<List<Chemical>> GetChemicalsAsync(bool excludeActive = false,
                                                        bool excludeDisabled = true,
                                                        bool excludeProposed = true,
                                                        bool excludeNotProposed = false,
                                                        Guid? forInstallation = null,
                                                        CancellationToken cancellationToken = default)
    {
        IQueryable<Chemical> chemicals = _dbContext.Chemicals.AsQueryable();

        chemicals = chemicals.Where(c => c.Disabled == excludeActive &&
                                         c.Disabled == excludeDisabled &&
                                         c.Tentative == excludeNotProposed);
        if (excludeProposed)
        {
            if (forInstallation is not null)
            {
                chemicals = chemicals.Where(c => c.Tentative == false || c.ProposedByInstallationId == forInstallation);
            }
            else
            {
                chemicals = chemicals.Where(c => c.Tentative == false);
            }
        }

        return await chemicals.ToListAsync(cancellationToken);
    }

    public async Task<Chemical> GetByNameAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Chemicals.FirstOrDefaultAsync(c => c.Name == name, cancellationToken);
    }

    public async Task<Chemical> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Chemicals.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Chemicals.AnyAsync(c => c.Name == name, cancellationToken);
    }

    public async Task InsertAsync(Chemical chemical, CancellationToken cancellationToken = default)
    {
        await _dbContext.Chemicals.AddAsync(chemical, cancellationToken);
    }

    public void Update(Chemical chemical)
    {
        _dbContext.Chemicals.Update(chemical);
    }

    public async Task<List<ShipmentChemical>> GetShipmentChemicalsByChemicalIdAsync(Guid chemicalId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShipmentChemicals.Include(s => s.Shipment).Where(sc => sc.ChemicalId == chemicalId).ToListAsync(cancellationToken);
    }
}
