using Domain.Attachments;

namespace Application.Common.Repositories;

public interface IAttachmentsRepository
{
    void Delete(Attachment attachment);
    Task<List<Attachment>> GetAttachmentsByShipmentIdAsync(Guid shipmentId, CancellationToken cancellationToken = default);
    Task<Attachment> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task InsertAsync(Attachment attachment, CancellationToken cancellationToken = default);
}