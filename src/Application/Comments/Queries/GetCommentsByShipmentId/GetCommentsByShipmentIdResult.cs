using Domain.Comments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Queries.GetCommentsByShipmentId;

public sealed record GetCommentsByShipmentIdResult
{
    public int RecordsCount { get; set; }
    public List<CommentResult> Comments { get; set; }

    public static GetCommentsByShipmentIdResult Map(List<Comment> comments)
    {
        List<CommentResult> result = new();
        foreach (var comment in comments)
        {
            result.Add(new CommentResult
            {
                Id = comment.Id,
                CommentText = comment.CommentText,
                ShipmentId = comment.ShipmentId.Value,
                Updated = comment.Updated,
                UpdatedBy = comment.UpdatedBy,
                UpdatedByName = comment.UpdatedByName
            });
        }

        return new GetCommentsByShipmentIdResult
        {
            RecordsCount = result.Count,
            Comments = result
        };
    }
}

public sealed record CommentResult
{
    public Guid Id { get; set; }
    public string CommentText { get; set; }
    public Guid ShipmentId { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
}
