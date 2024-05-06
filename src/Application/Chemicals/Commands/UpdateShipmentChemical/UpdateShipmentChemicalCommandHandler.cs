using Application.Common;
using Application.Common.Repositories;
using Domain.ShipmentChemicals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.UpdateShipmentChemical;

public sealed class UpdateShipmentChemicalCommandHandler : ICommandHandler<UpdateShipmentChemicalCommand, Result<Guid>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateShipmentChemicalCommandHandler(IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork)
    {
        _shipmentsRepository = shipmentsRepository;
        _unitOfWork = unitOfWork;

    }
    public async Task<Result<Guid>> HandleAsync(UpdateShipmentChemicalCommand command, CancellationToken cancellationToken = default)
    {
        List<string> errors = new();
        if (!ValidationUtils.IsCorrectMeasureUnit(command.MeasureUnit))
        {
            errors.Add("Invalid measure unit");
            return Result<Guid>.Failed(errors);
        }

        ShipmentChemical shipmentChemical = await _shipmentsRepository.GetShipmentChemicalAsync(command.ShipmentId, command.ChemicalId, cancellationToken);
        if (shipmentChemical is null)
        {
            return Result<Guid>.NotFound(new List<string> { "Chemical not found in shipment" });
        }

        shipmentChemical.Update(command.Amount,
                                command.MeasureUnit,
                                command.CalculatedBiocides,
                                command.CalculatedNitrogen,
                                command.CalculatedToc,
                                command.CalculatedBiocidesUnrinsed,
                                command.CalculatedNitrogenUnrinsed,
                                command.CalculatedTocUnrinsed,
                                command.UpdatedBy,
                                command.UpdatedByName);

        //TODO: Update shipment as well? Yes

        _shipmentsRepository.UpdateShipmentChemical(shipmentChemical);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        return Result<Guid>.Success(shipmentChemical.Id);
    }
}
