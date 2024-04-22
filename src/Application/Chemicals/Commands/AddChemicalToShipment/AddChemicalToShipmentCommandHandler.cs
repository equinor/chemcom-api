using Application.Common;
using Application.Common.Repositories;
using Domain.ShipmentChemicals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.AddChemicalToShipment;

public sealed class AddChemicalToShipmentCommandHandler : ICommandHandler<AddChemicalToShipmentCommand, Result<Guid>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public AddChemicalToShipmentCommandHandler(IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork)
    {
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Guid>> HandleAsync(AddChemicalToShipmentCommand command)
    {
        List<string> errors = new();
        if (!IsCorrectMeasureUnit(command.MeasureUnit))
        {
            errors.Add("Invalid measure unit");
            return Result<Guid>.Failed(errors);
        }

        ShipmentChemical shipmentChemical = await _shipmentsRepository.GetShipmentChemicalAsync(command.ShipmentId, command.ChemicalId);
        if (shipmentChemical is not null)
        {
            return Result<Guid>.Failed(new List<string> { "Chemical already added to shipment" });
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

        await _shipmentsRepository.AddShipmentChemicalAsync(shipmentChemical);
        await _unitOfWork.CommitChangesAsync();
        return Result<Guid>.Success(shipmentChemical.Id);
    }

    private bool IsCorrectMeasureUnit(string measureUnit)
    {
        return measureUnit == MeasureUnit.Kilogram ||
               measureUnit == MeasureUnit.Litre ||
               measureUnit == MeasureUnit.Tonn ||
               measureUnit == MeasureUnit.CubicMeter;
    }
}
