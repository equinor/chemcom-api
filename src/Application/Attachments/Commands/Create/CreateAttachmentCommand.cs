using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Commands.Create;

public sealed record CreateAttachmentCommand : ICommand
{
    public CreateAttachmentCommand(Guid shipmentId, string path, string extension, string mimeType, byte[] fileContents, string updatedBy, string updatedByName)
    {
        ShipmentId = shipmentId;
        Path = path;
        Extension = extension;
        MimeType = mimeType;
        FileContents = fileContents;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedByName;
    }

    public Guid ShipmentId { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public byte[] FileContents { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
}
