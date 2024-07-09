using Application.Common;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Decline;

public sealed record DeclineShipmentCommand : ICommand
{
    public Guid ShipmentId { get; set; }
    public User User { get; set; }
}
