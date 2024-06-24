using Application.Comments.Services;
using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Domain.Attachments;
using Domain.Shipments;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Commands.Create;

public sealed class CreateAttachmentCommandHandler : ICommandHandler<CreateAttachmentCommand, Result<CreateAttachmentResult>>
{
    private readonly IAttachmentsRepository _attachmentsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IFileUploadService _fileUploadService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<CreateAttachmentCommandHandler> _logger;

    public CreateAttachmentCommandHandler(IAttachmentsRepository attachmentsRepository,
        IShipmentsRepository shipmentsRepository,
        IUnitOfWork unitOfWork, IFileUploadService
        fileUploadService, ILogger<CreateAttachmentCommandHandler> logger)
    {
        _attachmentsRepository = attachmentsRepository;
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
        _fileUploadService = fileUploadService;
        _logger = logger;
    }

    public async Task<Result<CreateAttachmentResult>> HandleAsync(CreateAttachmentCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);
        if (shipment is null)
        {
            return Result<CreateAttachmentResult>.NotFound(new List<string> { ShipmentValidationErrors.ShipmentNotFoundText });
        }

        bool isFileUploadSuccessful = await _fileUploadService.UploadAsync(
                                                command.ShipmentId.ToString().ToLower(),
                                                command.Path,
                                                command.FileContents,
                                                cancellationToken);
        if (!isFileUploadSuccessful)
        {
            return Result<CreateAttachmentResult>.Failed(new List<string> { ShipmentValidationErrors.FileUploadFailedText });
        }

        Attachment attachment = new(command.ShipmentId,
                                                command.Path,
                                                command.ContentType,
                                                command.Extension,
                                                command.User.Email,
                                                command.User.Name);
        shipment.SetUpdatedInfo(command.User.Email, command.User.Name);
        _shipmentsRepository.Update(shipment);
        await _attachmentsRepository.InsertAsync(attachment, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Attachment for shipment {shipmentId} added successfully", shipment.Id);
        return Result<CreateAttachmentResult>.Success(new CreateAttachmentResult(attachment.Id, shipment.Id));
    }
}
