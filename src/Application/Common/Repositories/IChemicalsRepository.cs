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
    Task<bool> ExistsAsync(string name);
    Task<Chemical> GetByIdAsync(Guid id);
    Task<Chemical> GetByNameAsync(string name);
    Task<List<Chemical>> GetChemicalsAsync(bool excludeActive = false, bool excludeDisabled = true, bool excludeProposed = true, bool excludeNotProposed = false, Guid? forInstallation = null);
    Task<List<ShipmentChemical>> GetShipmentChemicalsByChemicalIdAsync(Guid chemicalId);
    Task InsertAsync(Chemical chemical);
    void Update(Chemical chemical);
}
