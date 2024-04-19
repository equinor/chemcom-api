using System;

namespace ChemDec.Api.Endpoints.Comments.Create;

public sealed record CreateCommentRequest
{
    public string CommentText { get; set; }
}
