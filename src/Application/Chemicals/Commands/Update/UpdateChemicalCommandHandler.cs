using Application.Common;
using Application.Common.Repositories;
using Domain.Chemicals;
using Domain.ShipmentChemicals;

namespace Application.Chemicals.Commands.Update;

public sealed class UpdateChemicalCommandHandler : ICommandHandler<UpdateChemicalCommand, Result<UpdateChemicalResult>>
{
    private readonly IChemicalsRepository _chemicalsRepository;
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateChemicalCommandHandler(IChemicalsRepository chemicalsRepository, IUnitOfWork unitOfWork, IShipmentsRepository shipmentsRepository)
    {
        _chemicalsRepository = chemicalsRepository;
        _unitOfWork = unitOfWork;
        _shipmentsRepository = shipmentsRepository;
    }
    public async Task<Result<UpdateChemicalResult>> HandleAsync(UpdateChemicalCommand command, CancellationToken cancellationToken = default)
    {
        Chemical chemical = await _chemicalsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (chemical is null)
        {
            return Result<UpdateChemicalResult>.NotFound(new List<string> { "Chemical not found" });
        }

        List<string> errors = new();

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            errors.Add("Chemical name must be set");
        }

        if (string.IsNullOrWhiteSpace(command.Description))
        {
            errors.Add("Chemical description must be set");
        }

        if (command.Name.Contains(";"))
        {
            errors.Add("Chemical name cannot contain semicolons");
        }

        if (command.Description.Contains(";"))
        {
            errors.Add("Chemical description cannot contain semicolons");
        }
       
        if (errors.Any())
        {
            return Result<UpdateChemicalResult>.Failed(errors);
        }        

        List<ShipmentChemical> shipmentChemicals = await _chemicalsRepository.GetShipmentChemicalsByChemicalIdAsync(command.Id, cancellationToken);

        if (chemical.TocWeight != command.TocWeight || chemical.NitrogenWeight != command.NitrogenWeight || chemical.Density != command.Density)
        {
            foreach (ShipmentChemical shipmentChemical in shipmentChemicals)
            {
                if (shipmentChemical.MeasureUnit == MeasureUnit.Kilogram)
                {
                    shipmentChemical.CalculatedWeight = shipmentChemical.Amount;
                }

                if (shipmentChemical.MeasureUnit == MeasureUnit.Tonn)
                {
                    shipmentChemical.CalculatedWeight = shipmentChemical.Amount * 1000;
                }

                if (shipmentChemical.MeasureUnit == MeasureUnit.Litre)
                {
                    shipmentChemical.CalculatedWeight = shipmentChemical.Amount * chemical.Density;
                }

                if (shipmentChemical.MeasureUnit == MeasureUnit.CubicMetre)
                {
                    shipmentChemical.CalculatedWeight = shipmentChemical.Amount * chemical.Density * 1000;
                }

                shipmentChemical.CalculatedWeightUnrinsed = shipmentChemical.CalculatedWeight;
                shipmentChemical.CalculatedNitrogenUnrinsed = shipmentChemical.CalculatedWeight * chemical.NitrogenWeight / 100;
                shipmentChemical.CalculatedTocUnrinsed = shipmentChemical.CalculatedWeight * chemical.TocWeight / 100;
                shipmentChemical.CalculatedBiocidesUnrinsed = shipmentChemical.CalculatedWeight * chemical.BiocideWeight / 100;
                shipmentChemical.CalculatedWeight = shipmentChemical.CalculatedWeight * ((100 - shipmentChemical.Shipment.RinsingOffshorePercent) / 100);
                shipmentChemical.CalculatedNitrogen = shipmentChemical.CalculatedWeight * chemical.NitrogenWeight / 100;
                shipmentChemical.CalculatedToc = shipmentChemical.CalculatedWeight * chemical.TocWeight / 100;
                shipmentChemical.CalculatedBiocides = shipmentChemical.CalculatedWeight * chemical.BiocideWeight / 100;

                _shipmentsRepository.UpdateShipmentChemical(shipmentChemical);
            }
        }


        chemical = UpdateChemicalCommand.Map(command, chemical);
        _chemicalsRepository.Update(chemical);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        return Result<UpdateChemicalResult>.Success(UpdateChemicalResult.Map(chemical));
    }
}
