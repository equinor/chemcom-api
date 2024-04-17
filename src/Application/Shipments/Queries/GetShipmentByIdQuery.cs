using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Queries;

public sealed record GetShipmentByIdQuery : IQuery
{
    public GetShipmentByIdQuery(Guid id)
    {
        Id = id;
    }
    public Guid Id { get; set; }
}
