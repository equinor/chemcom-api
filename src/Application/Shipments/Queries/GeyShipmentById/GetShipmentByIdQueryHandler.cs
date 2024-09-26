using Application.Common;
using Application.Common.Repositories;
using Domain.Installations;
using Domain.ShipmentParts;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Queries.GeyShipmentById;

public sealed class GetShipmentByIdQueryHandler : IQueryHandler<GetShipmentByIdQuery, Result<GetShipmentByIdQueryResult>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IShipmentPartsRepository _shipmentPartsRepository;
    private readonly IInstallationsRepository _installationsRepository;

    public GetShipmentByIdQueryHandler(IShipmentsRepository shipmentsRepository,
        IShipmentPartsRepository shipmentPartsRepository,
        IInstallationsRepository installationsRepository)
    {
        _shipmentsRepository = shipmentsRepository;
        _shipmentPartsRepository = shipmentPartsRepository;
        _installationsRepository = installationsRepository;
    }

    public async Task<Result<GetShipmentByIdQueryResult>> ExecuteAsync(GetShipmentByIdQuery query, CancellationToken cancellationToken = default)
    {
        List<string> errors = new();
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(query.Id, cancellationToken);

        if (shipment is null)
        {
            errors.Add("Shipment not found");
            return Result<GetShipmentByIdQueryResult>.NotFound(errors);
        }

        Installation installation = await _installationsRepository.GetByIdAsync(shipment.SenderId, cancellationToken);

        List<ShipmentPart> shipmentParts = await _shipmentPartsRepository.GetByShipmentIdAsync(shipment.Id, cancellationToken);
        GetShipmentByIdQueryResult queryResult = GetShipmentByIdQueryResult.Map(shipment, installation, shipmentParts);
        return Result<GetShipmentByIdQueryResult>.Success(queryResult);
    }
}
