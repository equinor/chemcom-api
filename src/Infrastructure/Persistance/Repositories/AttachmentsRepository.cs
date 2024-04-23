using Application.Common.Repositories;
using Domain.Attachments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories;

public sealed class AttachmentsRepository : IAttachmentsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AttachmentsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertAsync(Attachment attachment)
    {
        await _dbContext.Attachments.AddAsync(attachment);
    }

    public void Delete(Attachment attachment)
    {
        _dbContext.Attachments.Remove(attachment);
    }

    public async Task<List<Attachment>> GetAttachmentsByShipmentId(Guid shipmentId)
    {
        return await _dbContext.Attachments.Where(x => x.ShipmentId == shipmentId).ToListAsync();
    }

    public async Task<Attachment> GetByIdAsync(Guid id)
    {
        return await _dbContext.Attachments.FindAsync(id);
    }
}
