using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Application.Shipments.Commands.Approve;
using Domain.Shipments;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Decline;

public sealed class DeclineShipmentCommandHandler : ICommandHandler<DeclineShipmentCommand, Result<bool>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<ApproveShipmentCommandHandler> _logger;


    //TODO: Validation
    public DeclineShipmentCommandHandler(IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork, ILogger<ApproveShipmentCommandHandler> logger)
    {
        _shipmentsRepository = shipmentsRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<bool>> HandleAsync(DeclineShipmentCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);

        if (shipment is null)
        {
            return Result<bool>.NotFound([ShipmentValidationErrors.ShipmentNotFoundText]);
        }

        shipment.SetStatus(ShipmentStatuses.Declined);
        shipment.SetUpdatedInfo(command.User.Email, command.User.Name);
        _shipmentsRepository.Update(shipment);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Shipment with id: {ShipmentId} declined", shipment.Id);
        return Result<bool>.Success(true);
    }
}
