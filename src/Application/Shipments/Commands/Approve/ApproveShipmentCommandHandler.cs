using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Application.Shipments.Commands.Update;
using Domain.Shipments;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Approve;

public sealed class ApproveShipmentCommandHandler : ICommandHandler<ApproveShipmentCommand, Result<bool>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private ILogger<ApproveShipmentCommandHandler> _logger;

    public ApproveShipmentCommandHandler(IShipmentsRepository shipmentsRepository, IUnitOfWork unitOfWork, ILogger<ApproveShipmentCommandHandler> logger)
    {
        _shipmentsRepository = shipmentsRepository;
        _logger = logger;
        _unitOfWork = unitOfWork;
    }

    //TODO: Validation
    public async Task<Result<bool>> HandleAsync(ApproveShipmentCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);

        if (shipment is null)
        {
            return Result<bool>.NotFound([ShipmentValidationErrors.ShipmentNotFoundText]);
        }

        shipment.SetStatus(ShipmentStatuses.Approved);
        shipment.SetUpdatedInfo(command.User.Email, command.User.Name);
        _shipmentsRepository.Update(shipment);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Shipment with id: {ShipmentId} approved", shipment.Id);
        return Result<bool>.Success(true);
    }
}
