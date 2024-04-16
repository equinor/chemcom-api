using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Attachments;

public class Attachment
{
    public Attachment(Guid shipmentId, string path, string contentType, string extension, string updatedBy, string updatedByName)
    {
        Id = Guid.NewGuid();
        ShipmentId = shipmentId;
        Path = path;
        MimeType = contentType;
        Extension = extension;
        Updated = DateTime.Now;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedByName;
    }
    public Guid Id { get; init; }
    public Guid ShipmentId { get; init; }
    public Shipment Shipment { get; set; }
    public string Path { get; set; }
    public string Extension { get; set; }
    public string MimeType { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }

    private Attachment()
    { }
}
