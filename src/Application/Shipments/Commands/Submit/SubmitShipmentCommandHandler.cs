using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Submit;

public sealed class SubmitShipmentCommandHandler : ICommandHandler<SubmitShipmentCommand, Result<bool>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public SubmitShipmentCommandHandler(IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork)
    {
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<bool>> HandleAsync(SubmitShipmentCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);
        if (shipment == null)
        {
            return Result<bool>.NotFound(new List<string> { "Shipment not found" });
        }

        if (shipment.Status != ShipmentStatuses.Draft)
        {
            return Result<bool>.Failed(new List<string> { "Can't re-submit shipment" });
        }

        if (command.TakePrecaution)
        {
            shipment.Precautions = command.Precautions;
        }

        if (command.HeightenedLra)
        {
            shipment.Pb210 = command.Pb210;
            shipment.Ra226 = command.Ra226;
            shipment.Ra228 = command.Ra228;
        }

        shipment.AvailableForDailyContact = command.AvailableForDailyContact;
        shipment.HeightenedLra = command.HeightenedLra;
        shipment.TakePrecaution = command.TakePrecaution;
        shipment.Updated = DateTime.Now;
        shipment.UpdatedBy = command.UpdatedBy;
        shipment.UpdatedByName = command.UpdatedByName;
        shipment.Status = ShipmentStatuses.Submitted;

        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}
