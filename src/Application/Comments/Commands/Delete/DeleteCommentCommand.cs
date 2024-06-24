using Application.Common;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands.Delete;

public sealed record DeleteCommentCommand : ICommand
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public User User { get; set; }

    public DeleteCommentCommand(Guid id, Guid shipmentId, User user)
    {
        Id = id;
        ShipmentId = shipmentId;
        User = user;
    }
}
