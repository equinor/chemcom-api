using Application.Common.Repositories;
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

    public async Task<Shipment> GetByIdAsync(Guid id)
    {
        return await _dbContext.Shipments.FindAsync(id);
    }
}
