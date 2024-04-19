using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Queries.GetCommentsByShipmentId;

public sealed record GetCommentsByShipmentIdQuery : IQuery
{
    public GetCommentsByShipmentIdQuery(Guid shipmentId)
    {
        ShipmentId = shipmentId;
    }
    public Guid ShipmentId { get; set; }
}
