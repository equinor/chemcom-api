using Application.Common;
using Application.Common.Repositories;
using Domain.ShipmentParts;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Queries;

public sealed class GetShipmentByIdQueryHandler : IQueryHandler<GetShipmentByIdQuery, Result<GetShipmentByIdQueryResult>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IShipmentPartsRepository _shipmentPartsRepository;

    public GetShipmentByIdQueryHandler(IShipmentsRepository shipmentsRepository, IShipmentPartsRepository shipmentPartsRepository)
    {
        _shipmentsRepository = shipmentsRepository;
        _shipmentPartsRepository = shipmentPartsRepository;

    }

    public async Task<Result<GetShipmentByIdQueryResult>> ExecuteAsync(GetShipmentByIdQuery query)
    {
        Result<GetShipmentByIdQueryResult> result = new();
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(query.Id);

        if (shipment is null)
        {
            result.Errors.Add("Shipment not found");
            result.Status = ResultStatusConstants.NotFound;
            return result;
        }

        List<ShipmentPart> shipmentParts = await _shipmentPartsRepository.GetByShipmentIdAsync(shipment.Id);
        GetShipmentByIdQueryResult queryResult = GetShipmentByIdQueryResult.Map(shipment, shipmentParts);
        result.Status = ResultStatusConstants.Success;
        result.Data = queryResult;
        return result;
    }
}
