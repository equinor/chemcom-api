using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands.Create;

public sealed record CreateCommentCommand : ICommand
{  
    public CreateCommentCommand(string commentText, Guid shipmentId, string updatedBy, string updatedByName)
    {
        CommentText = commentText;
        ShipmentId = shipmentId;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedByName;
    }

    public string CommentText { get; set; }
    public Guid ShipmentId { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
}
