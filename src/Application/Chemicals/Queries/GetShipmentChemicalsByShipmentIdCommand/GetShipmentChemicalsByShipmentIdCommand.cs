using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Queries.GetShipmentChemicalsByShipmentIdCommand;

public sealed record GetShipmentChemicalsByShipmentIdCommand : IQuery
{
    public GetShipmentChemicalsByShipmentIdCommand(Guid shipmentId)
    {
        ShipmentId = shipmentId;
    }
    public Guid ShipmentId { get; set; }
}
