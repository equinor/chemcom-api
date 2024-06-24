using Application.Common;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands.Create;

public sealed record CreateCommentCommand : ICommand
{
    public CreateCommentCommand(string commentText, Guid shipmentId, User user)
    {
        CommentText = commentText;
        ShipmentId = shipmentId;
        User = user;
    }

    public string CommentText { get; set; }
    public Guid ShipmentId { get; set; }
    public User User { get; set; }
}
