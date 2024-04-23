using Application.Attachments.Queries.GetAttachmentsByShipmentId;
using Application.Common;
using Application.Common.Repositories;
using Domain.Attachments;
using Domain.Shipments;
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

    public DeleteAttachmentCommandHandler(IAttachmentsRepository attachmentsRepository, IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork)
    {
        _attachmentsRepository = attachmentsRepository;
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> HandleAsync(DeleteAttachmentCommand command)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId);
        if (shipment is null)
        {
            return Result<bool>.NotFound(new List<string> { "Shipment not found" });
        }

        Attachment attachment = await _attachmentsRepository.GetByIdAsync(command.AttachmentId);
        if (attachment is null)
        {
            return Result<bool>.NotFound(new List<string> { "Attachment not found" });
        }

        _attachmentsRepository.Delete(attachment);
        await _unitOfWork.CommitChangesAsync();
        return Result<bool>.Success(true);
    }
}
