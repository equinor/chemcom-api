using Domain.Attachments;

namespace Application.Common.Repositories;

public interface IAttachmentsRepository
{
    void Delete(Attachment attachment);
    Task<List<Attachment>> GetAttachmentsByShipmentId(Guid shipmentId);
    Task<Attachment> GetByIdAsync(Guid id);
    Task InsertAsync(Attachment attachment);
}