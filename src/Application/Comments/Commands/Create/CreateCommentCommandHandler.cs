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

namespace Application.Comments.Commands.Create;

public sealed class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Result<Guid>>
{
    private readonly ICommentsRepository _commentsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateCommentCommandHandler> _logger;
    public CreateCommentCommandHandler(ICommentsRepository commentsRepository,
        IShipmentsRepository shipmentsRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateCommentCommandHandler> logger)
    {
        _commentsRepository = commentsRepository;
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<Guid>> HandleAsync(CreateCommentCommand command, CancellationToken cancellationToken = default)
    {
        List<string> errors = new();

        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);

        if (shipment is null)
        {
            errors.Add("Shipment not found");
            return Result<Guid>.NotFound(errors);
        }

        if (string.IsNullOrWhiteSpace(command.CommentText))
        {
            errors.Add("Comment text is required");
        }

        if (errors.Any())
        {
            return Result<Guid>.Failed(errors);
        }

        Comment comment = new(command.CommentText, command.ShipmentId, command.User);
        comment.SetNewId();

        shipment.SetUpdatedInfo(command.User.Email, command.User.Name);

        await _commentsRepository.InsertAsync(comment, cancellationToken);
        _shipmentsRepository.Update(shipment);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Comment with id {commentId} for shipment {shipmentId} has been created", comment.Id, shipment.Id);
        return Result<Guid>.Success(comment.Id);
    }
}
