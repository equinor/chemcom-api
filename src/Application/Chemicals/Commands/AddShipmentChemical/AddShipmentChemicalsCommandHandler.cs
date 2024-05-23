using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Domain.ShipmentChemicals;
using Domain.Shipments;

namespace Application.Chemicals.Commands.AddShipmentChemical;

public sealed class AddShipmentChemicalsCommandHandler : ICommandHandler<AddShipmentChemicalsCommand, Result<List<Guid>>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddShipmentChemicalsCommandHandler(IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork)
    {
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<List<Guid>>> HandleAsync(AddShipmentChemicalsCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);
        if (shipment is null)
        {
            return Result<List<Guid>>.NotFound(new List<string> { ShipmentValidationErrors.ShipmentNotFoundText });
        }

        List<string> errors = new();

        foreach (ShipmentChemicalItem item in command.ShipmentChemicalItems)
        {
            if (!ValidationUtils.IsCorrectMeasureUnit(item.MeasureUnit))
            {
                errors.Add(ShipmentValidationErrors.InvalidMeasureUnitText);
            }
        }

        if (errors.Any())
        {
            return Result<List<Guid>>.Failed(errors);
        }

        List<Guid> updatedList = new();
        foreach (ShipmentChemicalItem item in command.ShipmentChemicalItems)
        {
            ShipmentChemical shipmentChemical = await _shipmentsRepository.GetShipmentChemicalAsync(command.ShipmentId, item.ChemicalId, cancellationToken);

            if (shipmentChemical is not null)
            {
                shipmentChemical.Amount = item.Amount;
                shipmentChemical.MeasureUnit = item.MeasureUnit;
                shipmentChemical.CalculatedBiocides = item.CalculatedBiocides;
                shipmentChemical.CalculatedNitrogen = item.CalculatedNitrogen;
                shipmentChemical.CalculatedToc = item.CalculatedToc;
                shipmentChemical.CalculatedBiocidesUnrinsed = item.CalculatedBiocidesUnrinsed;
                shipmentChemical.CalculatedNitrogenUnrinsed = item.CalculatedNitrogenUnrinsed;
                shipmentChemical.CalculatedTocUnrinsed = item.CalculatedTocUnrinsed;
                shipmentChemical.Updated = DateTime.Now;
                shipmentChemical.UpdatedBy = command.UpdatedBy;
                shipmentChemical.UpdatedByName = command.UpdatedByName;
                _shipmentsRepository.UpdateShipmentChemical(shipmentChemical);
            }
            else
            {
                shipmentChemical = new ShipmentChemical(item.ChemicalId,
                                                               command.ShipmentId,
                                                               item.Amount,
                                                               item.MeasureUnit,
                                                               item.CalculatedBiocides,
                                                               item.CalculatedNitrogen,
                                                               item.CalculatedToc,
                                                               item.CalculatedBiocidesUnrinsed,
                                                               item.CalculatedNitrogenUnrinsed,
                                                               item.CalculatedTocUnrinsed,
                                                               command.UpdatedBy,
                                                               command.UpdatedByName);
                await _shipmentsRepository.AddShipmentChemicalAsync(shipmentChemical, cancellationToken);
            }

            updatedList.Add(shipmentChemical.Id);
        }

        shipment.SetUpdatedInfo(command.UpdatedBy, command.UpdatedByName);
        _shipmentsRepository.Update(shipment);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        return Result<List<Guid>>.Success(updatedList);
    }
}
