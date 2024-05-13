using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.DeleteShipmentChemical;

public sealed record DeleteShipmentChemicalCommand : ICommand
{
    public DeleteShipmentChemicalCommand(Guid id, Guid shipmentId, string updatedBy, string updatedByName)
    {
        Id = id;
        ShipmentId = shipmentId;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedByName;
    }

    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
}
