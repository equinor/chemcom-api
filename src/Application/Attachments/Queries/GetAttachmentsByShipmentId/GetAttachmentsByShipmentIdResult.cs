using Domain.Attachments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Queries.GetAttachmentsByShipmentId;

public sealed record GetAttachmentsByShipmentIdResult
{
    public int RecordsCount { get; set; }
    public List<AttachmentResult> Attachments { get; set; }

    public static GetAttachmentsByShipmentIdResult Map(List<Attachment> attachments)
    {
        List<AttachmentResult> attachmentResults = new();
        foreach (var attachment in attachments)
        {
            attachmentResults.Add(new AttachmentResult
            {
                Id = attachment.Id,
                ShipmentId = attachment.ShipmentId,
                Path = attachment.Path,
                Extension = attachment.Extension,
                MimeType = attachment.MimeType,
                Updated = attachment.Updated,
                UpdatedBy = attachment.UpdatedBy,
                UpdatedByName = attachment.UpdatedByName
            });
        }
        return new GetAttachmentsByShipmentIdResult
        {
            RecordsCount = attachments.Count,
            Attachments = attachmentResults
        };
    }
}

public sealed record AttachmentResult
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
}
