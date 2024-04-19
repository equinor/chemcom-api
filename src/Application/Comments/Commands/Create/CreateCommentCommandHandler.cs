using Application.Common;
using Application.Common.Repositories;
using Domain.Comments;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands.Create;

public sealed class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Result<CreateCommentResult>>
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateCommentCommandHandler(ICommentsRepository commentsRepository, IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork)
    {
        _commentsRepository = commentsRepository;
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<CreateCommentResult>> HandleAsync(CreateCommentCommand command)
    {
        List<string> errors = new();

        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId);

        if (shipment is null)
        {
            errors.Add("Shipment not found");
            return Result<CreateCommentResult>.NotFound(errors);
        }

        if (string.IsNullOrWhiteSpace(command.CommentText))
        {
            errors.Add("Comment text is required");
        }

        if (errors.Any())
        {
            return Result<CreateCommentResult>.Failed(errors);
        }

        Comment comment = new(command.CommentText, command.ShipmentId, command.UpdatedBy, command.UpdatedByName);
        comment.SetNewId();
        await _commentsRepository.InsertAsync(comment);
        await _unitOfWork.CommitChangesAsync();

        return Result<CreateCommentResult>.Success(
            new CreateCommentResult(
                comment.Id,
                comment.CommentText,
                comment.ShipmentId.Value,
                comment.Updated,
                comment.UpdatedBy,
                comment.UpdatedByName));
    }
}
