using Application.Comments.Services;
using Application.Common;
using Application.Common.Repositories;
using Domain.Attachments;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Commands.Create;

public sealed class CreateAttachmentCommandHandler : ICommandHandler<CreateAttachmentCommand, Result<bool>>
{
    private readonly IAttachmentsRepository _attachmentsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IFileUploadService _fileUploadService;
    private readonly IUnitOfWork _unitOfWork;

    public CreateAttachmentCommandHandler(IAttachmentsRepository attachmentsRepository,
        IShipmentsRepository shipmentsRepository,
        IUnitOfWork unitOfWork, IFileUploadService
        fileUploadService)
    {
        _attachmentsRepository = attachmentsRepository;
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
        _fileUploadService = fileUploadService;
    }

    public async Task<Result<bool>> HandleAsync(CreateAttachmentCommand command)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId);
        if (shipment is null)
        {
            return Result<bool>.NotFound(new List<string> { "Shipment not found" });
        }

        bool isFileUploadSuccessful = await _fileUploadService.UploadAsync(command.ShipmentId.ToString(), command.Path, command.FileContents);
        if (!isFileUploadSuccessful)
        {
            return Result<bool>.Failed(new List<string> { "File upload failed" });
        }

        Attachment attachment = new Attachment(command.ShipmentId,
                                                command.Path,
                                                command.MimeType,
                                                command.Extension,
                                                command.UpdatedBy,
                                                command.UpdatedByName);
        await _attachmentsRepository.InsertAsync(attachment);
        await _unitOfWork.CommitChangesAsync();

        return Result<bool>.Success(true);
    }
}
