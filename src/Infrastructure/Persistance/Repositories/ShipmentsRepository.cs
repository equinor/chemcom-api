using Application.Common.Repositories;
using Domain.ShipmentChemicals;
using Domain.Shipments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories;

public sealed class ShipmentsRepository : IShipmentsRepository
{
    private readonly ApplicationDbContext _dbContext;
    public ShipmentsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }
    public async Task InsertAsync(Shipment shipment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Shipments.AddAsync(shipment, cancellationToken);
    }

    public void Update(Shipment shipment)
    {
        _dbContext.Shipments.Update(shipment);
    }

    public async Task<Shipment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Shipments.FindAsync(id, cancellationToken);
    }

    public async Task<ShipmentChemical> GetShipmentChemicalAsync(Guid shipmentId, Guid chemicalId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShipmentChemicals
            .Where(sc => sc.ShipmentId == shipmentId && sc.ChemicalId == chemicalId)
            .FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<List<ShipmentChemical>> GetShipmentChemicalsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShipmentChemicals
            .Where(sc => sc.ShipmentId == shipmentId)
            .ToListAsync(cancellationToken);
    }

    public async Task AddShipmentChemicalAsync(ShipmentChemical shipmentChemical, CancellationToken cancellationToken = default)
    {
        await _dbContext.ShipmentChemicals.AddAsync(shipmentChemical, cancellationToken);
    }

    public void UpdateShipmentChemical(ShipmentChemical shipmentChemical)
    {
        _dbContext.ShipmentChemicals.Update(shipmentChemical);
    }
}
