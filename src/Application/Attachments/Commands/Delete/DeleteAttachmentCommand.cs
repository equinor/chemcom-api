using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Commands.Delete;

public sealed record DeleteAttachmentCommand : ICommand
{
    public Guid AttachmentId { get; set; }
    public Guid ShipmentId { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }

    public DeleteAttachmentCommand(Guid attachmentId, Guid shipmentId, string updatedBy, string updatedByName)
    {
        AttachmentId = attachmentId;
        ShipmentId = shipmentId;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedByName;
    }
}
