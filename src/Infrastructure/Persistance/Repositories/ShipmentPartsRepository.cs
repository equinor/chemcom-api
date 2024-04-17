using Application.Common.Repositories;
using Domain.ShipmentParts;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories;

public class ShipmentPartsRepository : IShipmentPartsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public ShipmentPartsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertAsync(ShipmentPart shipmentPart)
    {
        await _dbContext.ShipmentParts.AddAsync(shipmentPart);
    }

    public void Delete(ICollection<ShipmentPart> shipmentParts)
    {
        _dbContext.ShipmentParts.RemoveRange(shipmentParts);
    }

    public async Task<List<ShipmentPart>> GetByShipmentId(Guid shipmentId)
    {
        return await _dbContext.ShipmentParts.Where(x => x.ShipmentId == shipmentId).ToListAsync();
    }
}
