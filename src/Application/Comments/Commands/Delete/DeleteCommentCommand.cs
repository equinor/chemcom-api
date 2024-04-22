using Application.Common;
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

    public DeleteCommentCommand(Guid id, Guid shipmentId)
    {
        Id = id;
        ShipmentId = shipmentId;
    }
}
