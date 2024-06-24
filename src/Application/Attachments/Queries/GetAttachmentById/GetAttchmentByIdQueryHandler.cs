using Application.Attachments.Queries.GetAttachmentsByShipmentId;
using Application.Comments.Services;
using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Domain.Attachments;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Queries.GetAttachmentById;

public sealed class GetAttchmentByIdQueryHandler : IQueryHandler<GetAttachmentByIdQuery, Result<GetAttachmentByIdResult>>
{
    private readonly IAttachmentsRepository _attachmentsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IFileUploadService _fileUploadService;

    public GetAttchmentByIdQueryHandler(IShipmentsRepository shipmentsRepository, IAttachmentsRepository attachmentsRepository, IFileUploadService fileUploadService)
    {
        _shipmentsRepository = shipmentsRepository;
        _attachmentsRepository = attachmentsRepository;
        _fileUploadService = fileUploadService;
    }

    public async Task<Result<GetAttachmentByIdResult>> ExecuteAsync(GetAttachmentByIdQuery query, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(query.ShipmentId, cancellationToken);
        if (shipment is null)
        {
            return Result<GetAttachmentByIdResult>.NotFound([ShipmentValidationErrors.ShipmentNotFoundText]);
        }

        Attachment attachment = await _attachmentsRepository.GetByIdAsync(query.AttachmentId, cancellationToken);
        if (attachment is null)
        {
            return Result<GetAttachmentByIdResult>.NotFound([ShipmentValidationErrors.AttachmentNotFound]);
        }

        Stream stream = await _fileUploadService.GetAsync(query.ShipmentId.ToString().ToLower(), attachment.Path, cancellationToken);
        if (stream is null)
        {
            return Result<GetAttachmentByIdResult>.NotFound([ShipmentValidationErrors.AttachmentNotFound]);
        }

        return Result<GetAttachmentByIdResult>.Success(new GetAttachmentByIdResult(stream, attachment.Path, attachment.MimeType));
    }
}
