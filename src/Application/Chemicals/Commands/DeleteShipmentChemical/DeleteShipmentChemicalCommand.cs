using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.DeleteShipmentChemical;

public sealed record DeleteShipmentChemicalCommand : ICommand
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
}
