using Application.Common;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.DeleteShipmentChemical;

public sealed record DeleteShipmentChemicalCommand : ICommand
{
    public DeleteShipmentChemicalCommand(Guid id, Guid shipmentId, User user)
    {
        Id = id;
        ShipmentId = shipmentId;
        User = user;
    }

    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public User User { get; set; }
}
