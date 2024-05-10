using Application.Attachments.Queries.GetAttachmentsByShipmentId;
using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Domain.Attachments;
using Domain.Shipments;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Commands.Delete;

public sealed class DeleteAttachmentCommandHandler : ICommandHandler<DeleteAttachmentCommand, Result<bool>>
{
    private readonly IAttachmentsRepository _attachmentsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<DeleteAttachmentCommandHandler> _logger;

    public DeleteAttachmentCommandHandler(IAttachmentsRepository attachmentsRepository,
        IShipmentsRepository shipmentsRepository,
        IUnitOfWork unitOfWork,
        ILogger<DeleteAttachmentCommandHandler> logger)
    {
        _attachmentsRepository = attachmentsRepository;
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<bool>> HandleAsync(DeleteAttachmentCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);
        if (shipment is null)
        {
            return Result<bool>.NotFound(new List<string> { ShipmentValidationErrors.ShipmentNotFoundText });
        }

        Attachment attachment = await _attachmentsRepository.GetByIdAsync(command.AttachmentId, cancellationToken);
        if (attachment is null)
        {
            return Result<bool>.NotFound(new List<string> {ShipmentValidationErrors.AttachmentNotFound });
        }

        _attachmentsRepository.Delete(attachment);
        shipment.SetUpdatedInfo(command.UpdatedBy, command.UpdatedByName);
        _shipmentsRepository.Update(shipment);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Attachment for shipment {shipmentId} has been deleted", shipment.Id);
        return Result<bool>.Success(true);
    }
}
