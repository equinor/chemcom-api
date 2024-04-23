using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Queries.GetAttachmentsByShipmentId;

public sealed record GetAttachmentsByShipmentIdQuery : IQuery
{
    public Guid ShipmentId { get; set; }

    public GetAttachmentsByShipmentIdQuery(Guid shipmentId)
    {
        ShipmentId = shipmentId;
    }
}
