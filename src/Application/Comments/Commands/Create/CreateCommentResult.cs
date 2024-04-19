using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands.Create;

public sealed record CreateCommentResult
{
    public CreateCommentResult(Guid id, string commentText, Guid shipmentId, DateTime updated, string updatedBy, string updatedByName)
    {
        Id = id;
        CommentText = commentText;
        ShipmentId = shipmentId;
        Updated = updated;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedByName;
    }

    public Guid Id { get; set; }
    public string CommentText { get; set; }
    public Guid ShipmentId { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
}
