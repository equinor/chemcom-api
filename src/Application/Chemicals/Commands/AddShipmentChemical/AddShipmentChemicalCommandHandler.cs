using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Domain.ShipmentChemicals;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.AddShipmentChemical;

public sealed class AddShipmentChemicalCommandHandler : ICommandHandler<AddShipmentChemicalCommand, Result<Guid>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddShipmentChemicalCommandHandler(IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork)
    {
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Guid>> HandleAsync(AddShipmentChemicalCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);
        if (shipment is null)
        {
            return Result<Guid>.NotFound(new List<string> { ShipmentValidationErrors.ShipmentNotFoundText });
        }

        List<string> errors = new();
        if (!ValidationUtils.IsCorrectMeasureUnit(command.MeasureUnit))
        {
            errors.Add(ShipmentValidationErrors.InvalidMeasureUnitText);
            return Result<Guid>.Failed(errors);
        }

        ShipmentChemical shipmentChemical = await _shipmentsRepository.GetShipmentChemicalAsync(command.ShipmentId, command.ChemicalId, cancellationToken);
        if (shipmentChemical is not null)
        {
            return Result<Guid>.Failed(new List<string> { ShipmentValidationErrors.ChemicalAlreadyAddedText });
        }

        shipmentChemical = new ShipmentChemical(command.ChemicalId,
                                                command.ShipmentId,
                                                command.Amount,
                                                command.MeasureUnit,
                                                command.CalculatedBiocides,
                                                command.CalculatedNitrogen,
                                                command.CalculatedToc,
                                                command.CalculatedBiocidesUnrinsed,
                                                command.CalculatedNitrogenUnrinsed,
                                                command.CalculatedTocUnrinsed,
                                                command.UpdatedBy,
                                                command.UpdatedByName);
        shipment.SetUpdatedInfo(command.UpdatedBy, command.UpdatedByName);
        _shipmentsRepository.Update(shipment);
        await _shipmentsRepository.AddShipmentChemicalAsync(shipmentChemical, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        return Result<Guid>.Success(shipmentChemical.Id);
    }
}
