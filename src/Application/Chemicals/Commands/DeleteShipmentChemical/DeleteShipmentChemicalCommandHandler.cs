using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Application.Shipments.Commands.Update;
using Domain.ShipmentChemicals;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.DeleteShipmentChemical;

public sealed class DeleteShipmentChemicalCommandHandler : ICommandHandler<DeleteShipmentChemicalCommand, Result<bool>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteShipmentChemicalCommandHandler(IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork)
    {
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<bool>> HandleAsync(DeleteShipmentChemicalCommand command, CancellationToken cancellationToken = default)
    {
        List<string> errors = new();
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);

        if (shipment is null)
        {
            errors.Add(ShipmentValidationErrors.ShipmentNotFoundText);
            return Result<bool>.NotFound(errors);
        }

        ShipmentChemical shipmentChemical = await _shipmentsRepository.GetShipmentChemicalByIdAsync(command.Id, cancellationToken);

        if (shipmentChemical is null)
        {
            errors.Add(ShipmentValidationErrors.ShipmentChemicalNotFoundText);
            return Result<bool>.NotFound(errors);
        }

        _shipmentsRepository.DeleteShipmentChemical(shipmentChemical);
        shipment.SetUpdatedInfo(command.UpdatedBy, command.UpdatedByName);
        _shipmentsRepository.Update(shipment);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }
}