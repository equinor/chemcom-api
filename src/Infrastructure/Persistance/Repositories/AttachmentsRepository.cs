using Application.Common.Repositories;
using Domain.Attachments;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories;

public sealed class AttachmentsRepository : IAttachmentsRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AttachmentsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task InsertAsync(Attachment attachment, CancellationToken cancellationToken = default)
    {
        await _dbContext.Attachments.AddAsync(attachment, cancellationToken);
    }

    public void Delete(Attachment attachment)
    {
        _dbContext.Attachments.Remove(attachment);
    }

    public async Task<List<Attachment>> GetAttachmentsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Attachments.Where(x => x.ShipmentId == shipmentId).ToListAsync(cancellationToken);
    }

    public async Task<Attachment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Attachments.FindAsync(id, cancellationToken);
    }
}
