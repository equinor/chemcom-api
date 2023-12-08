using Application.Common.Repositories;
using Domain.ShipmentParts;

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
}
