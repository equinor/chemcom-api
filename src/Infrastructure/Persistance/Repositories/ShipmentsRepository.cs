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
    public async Task InsertAsync(Shipment shipment)
    {
        await _dbContext.Shipments.AddAsync(shipment);
    }

    public void Update(Shipment shipment)
    {
        _dbContext.Shipments.Update(shipment);
    }

    public async Task<Shipment> GetByIdAsync(Guid id)
    {
        return await _dbContext.Shipments.FindAsync(id);
    }

    public async Task<ShipmentChemical> GetShipmentChemicalAsync(Guid shipmentId, Guid chemicalId)
    {
        return await _dbContext.ShipmentChemicals
            .Where(sc => sc.ShipmentId == shipmentId && sc.ChemicalId == chemicalId)
            .FirstOrDefaultAsync();
    }

    public async Task<List<ShipmentChemical>> GetShipmentChemicalsByShipmentIdAsync(Guid shipmentId)
    {
        return await _dbContext.ShipmentChemicals
            .Where(sc => sc.ShipmentId == shipmentId)
            .ToListAsync();
    }

    public async Task AddShipmentChemicalAsync(ShipmentChemical shipmentChemical)
    {
        await _dbContext.ShipmentChemicals.AddAsync(shipmentChemical);
    }

    public void UpdateShipmentChemical(ShipmentChemical shipmentChemical)
    {
        _dbContext.ShipmentChemicals.Update(shipmentChemical);
    }
}
