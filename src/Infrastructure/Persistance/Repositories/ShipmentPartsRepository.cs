using Application.Common.Repositories;
using Domain.ShipmentParts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories;

public sealed class ShipmentPartsRepository : IShipmentPartsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ShipmentPartsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertAsync(ShipmentPart shipmentPart, CancellationToken cancellationToken = default)
    {
        await _dbContext.ShipmentParts.AddAsync(shipmentPart, cancellationToken);
    }

    public async Task InsertManyAsync(List<ShipmentPart> shipmentParts, CancellationToken cancellationToken = default)
    {
        await _dbContext.ShipmentParts.AddRangeAsync(shipmentParts, cancellationToken);
    }

    public void Delete(ICollection<ShipmentPart> shipmentParts)
    {
        _dbContext.ShipmentParts.RemoveRange(shipmentParts);
    }

    public async Task<List<ShipmentPart>> GetByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.ShipmentParts.Where(x => x.ShipmentId == shipmentId).ToListAsync(cancellationToken);
    }
}
