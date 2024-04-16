using Application.Common;
using Application.Shipments.Commands;
using Domain.Attachments;
using Domain.ShipmentChemicals;
using Domain.ShipmentParts;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Update;

public sealed class UpdateShipmentCommand : ShipmentCommandsBase, ICommand
{
    public Guid Id { get; set; }
}
