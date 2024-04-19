using Application.Common;
using Application.Common.Repositories;
using Domain.ShipmentChemicals;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Queries.GetShipmentChemicalsByShipmentIdCommand;

public sealed class GetShipmentChemicalsByShipmentIdCommandHandler :
    IQueryHandler<GetShipmentChemicalsByShipmentIdCommand, Result<GetShipmentChemicalsByShipmentIdResult>>
{
    private readonly IShipmentsRepository _shipmentsRepository;

    public GetShipmentChemicalsByShipmentIdCommandHandler(IShipmentsRepository shipmentsRepository)
    {
        _shipmentsRepository = shipmentsRepository;
    }
    public async Task<Result<GetShipmentChemicalsByShipmentIdResult>> ExecuteAsync(GetShipmentChemicalsByShipmentIdCommand command)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId);
        if (shipment is null)
        {
            return Result<GetShipmentChemicalsByShipmentIdResult>.NotFound(new List<string> { "Shipment not found" });
        }

        List<ShipmentChemical> shipmentChemicals = await _shipmentsRepository.GetShipmentChemicalsByShipmentIdAsync(command.ShipmentId);

        if (shipmentChemicals is null)
        {
            return Result<GetShipmentChemicalsByShipmentIdResult>.NotFound(new List<string> { "Shipment chemicals not found" });
        }

        List<ShipmentChemicalResult> results = new();
        foreach (ShipmentChemical shipmentChemical in shipmentChemicals)
        {
            results.Add(new ShipmentChemicalResult
            {
                Id = shipmentChemical.Id,
                ChemicalId = shipmentChemical.ChemicalId,
                ShipmentId = shipmentChemical.ShipmentId,
                Amount = shipmentChemical.Amount,
                MeasureUnit = shipmentChemical.MeasureUnit,
                CalculatedBiocides = shipmentChemical.CalculatedBiocides,
                CalculatedNitrogen = shipmentChemical.CalculatedNitrogen,
                CalculatedToc = shipmentChemical.CalculatedToc,
                CalculatedBiocidesUnrinsed = shipmentChemical.CalculatedBiocidesUnrinsed,
                CalculatedNitrogenUnrinsed = shipmentChemical.CalculatedNitrogenUnrinsed,
                CalculatedTocUnrinsed = shipmentChemical.CalculatedTocUnrinsed,
                UpdatedBy = shipmentChemical.UpdatedBy,
                UpdatedByName = shipmentChemical.UpdatedByName
            });
        }

        return Result<GetShipmentChemicalsByShipmentIdResult>.Success(new GetShipmentChemicalsByShipmentIdResult
        {
            ResultsCount = results.Count(),
            ShipmentChemicals = results
        });
    }
}
