using Application.Common;
using Application.Common.Repositories;
using Domain.Comments;
using Domain.Shipments;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Comments.Commands.Delete;

public sealed class DeleteCommentCommandHandler : ICommandHandler<DeleteCommentCommand, Result<bool>>
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteCommentCommandHandler> _logger;

    public DeleteCommentCommandHandler(ICommentsRepository commentsRepository,
        IShipmentsRepository shipmentsRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteCommentCommandHandler> logger)
    {
        _commentsRepository = commentsRepository;
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> HandleAsync(DeleteCommentCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);

        if (shipment is null)
        {
            return Result<bool>.NotFound(new List<string> { "Shipment not found" });
        }

        Comment comment = await _commentsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (comment is null)
        {
            return Result<bool>.NotFound(new List<string> { "Comment not found" });
        }

        shipment.SetUpdatedInfo(command.UpdatedBy, command.UpdatedByName);
        _shipmentsRepository.Update(shipment);
        _commentsRepository.Delete(comment);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Comment {commentId} for shipment {shipmentId} has been deleted", comment.Id, shipment.Id);
        return Result<bool>.Success(true);
    }
}
