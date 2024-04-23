using Domain.Chemicals;
using Domain.ShipmentChemicals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Repositories;

public interface IChemicalsRepository
{
    Task<bool> ExistsAsync(string name, CancellationToken cancellationToken = default);
    Task<Chemical> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<Chemical> GetByNameAsync(string name, CancellationToken cancellationToken = default);
    Task<List<Chemical>> GetChemicalsAsync(bool excludeActive = false,
        bool excludeDisabled = true,
        bool excludeProposed = true,
        bool excludeNotProposed = false,
        Guid? forInstallation = null,
        CancellationToken cancellationToken = default);
    Task<List<ShipmentChemical>> GetShipmentChemicalsByChemicalIdAsync(Guid chemicalId, CancellationToken cancellationToken = default);
    Task InsertAsync(Chemical chemical, CancellationToken cancellationToken = default);
    void Update(Chemical chemical);
}
