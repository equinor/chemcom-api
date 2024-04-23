using Application.Common;
using Application.Common.Repositories;
using Domain.Attachments;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Attachments.Queries.GetAttachmentsByShipmentId;

public sealed class GetAttachmentsByShipmentIdQueryHandler : IQueryHandler<GetAttachmentsByShipmentIdQuery, Result<GetAttachmentsByShipmentIdResult>>
{
    private readonly IAttachmentsRepository _attachmentsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;

    public GetAttachmentsByShipmentIdQueryHandler(IAttachmentsRepository attachmentsRepository, IShipmentsRepository shipmentsRepository)
    {
        _attachmentsRepository = attachmentsRepository;
        _shipmentsRepository = shipmentsRepository;
    }

    public async Task<Result<GetAttachmentsByShipmentIdResult>> ExecuteAsync(GetAttachmentsByShipmentIdQuery query)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(query.ShipmentId);
        if (shipment is null)
        {
            return Result<GetAttachmentsByShipmentIdResult>.NotFound(new List<string> { "Shipment not found" });
        }

        List<Attachment> attachments = await _attachmentsRepository.GetAttachmentsByShipmentId(query.ShipmentId);
        if (attachments is null)
        {
            return Result<GetAttachmentsByShipmentIdResult>.NotFound(new List<string> { "Attachments not found in the shipment" });
        }

        GetAttachmentsByShipmentIdResult result = GetAttachmentsByShipmentIdResult.Map(attachments);
        return Result<GetAttachmentsByShipmentIdResult>.Success(result);
    }
}
