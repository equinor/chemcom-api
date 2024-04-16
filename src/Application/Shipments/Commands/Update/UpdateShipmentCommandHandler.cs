using Application.Common;
using Application.Common.Repositories;
using Application.Shipments.Commands.Create;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Update;

public sealed class UpdateShipmentCommandHandler : ICommandHandler<UpdateShipmentCommand, bool>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public async Task<bool> HandleAsync(UpdateShipmentCommand command)
    {
        ShipmentDetails shipmentDetails = ShipmentCommandsBase.Map(command);
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.Id);
        shipment.Update(shipmentDetails);

        await _unitOfWork.CommitChangesAsync();
        return true;
    }
}
