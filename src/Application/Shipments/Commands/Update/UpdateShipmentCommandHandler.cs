using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Common.Repositories;
using Application.Shipments.Commands.Create;
using Domain.Installations;
using Domain.ShipmentParts;
using Domain.Shipments;
using Domain.Users;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Update;

public sealed class UpdateShipmentCommandHandler : ICommandHandler<UpdateShipmentCommand, Result<UpdateShipmentResult>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IInstallationsRepository _installationsRepository;
    private readonly IShipmentPartsRepository _shipmentPartsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<UpdateShipmentCommandHandler> _logger;
    public UpdateShipmentCommandHandler(IShipmentsRepository shipmentsRepository,
         IInstallationsRepository installationsRepository,
         IShipmentPartsRepository shipmentPartsRepository,
         IUnitOfWork unitOfWork,
         ILogger<UpdateShipmentCommandHandler> logger)
    {
        _shipmentsRepository = shipmentsRepository;
        _installationsRepository = installationsRepository;
        _shipmentPartsRepository = shipmentPartsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task<Result<UpdateShipmentResult>> HandleAsync(UpdateShipmentCommand command, CancellationToken cancellationToken = default)
    {
        //TODO: Validate receiver?
        List<string> errors = new();
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (shipment is null)
        {
            return Result<UpdateShipmentResult>.NotFound([ShipmentValidationErrors.ShipmentNotFoundText]);
        }
          
        if (command.SenderId == Guid.Empty)
        {           
            return Result<UpdateShipmentResult>.Failed([ShipmentValidationErrors.SenderRequiredText]);
        }

        Role role = command.User.Roles.FirstOrDefault(r => r.Installation != null && r.Installation.Id == command.SenderId);
        if (role is null)
        {           
            return Result<UpdateShipmentResult>.Failed([ShipmentValidationErrors.UserAccessForInstallationText]);
        }

        if (command.PlannedExecutionFrom is null)
        {
            errors.Add(ShipmentValidationErrors.PlannedExecutionFromDateRequiredText);
        }

        if (command.PlannedExecutionTo is null)
        {
            errors.Add(ShipmentValidationErrors.PlannedExecutionToDateRequiredText);
        }       

        if (errors.Any())
        {
            return Result<UpdateShipmentResult>.Failed(errors);
        }

        Installation installation = await _installationsRepository.GetByIdAsync(command.SenderId, cancellationToken);
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(installation.TimeZone);
        DateTime plannedExecutionFromLocal = TimeZoneInfo.ConvertTimeFromUtc(command.PlannedExecutionFrom.Value, timeZone);
        DateTime plannedExecutionToLocal = TimeZoneInfo.ConvertTimeFromUtc(command.PlannedExecutionTo.Value, timeZone);

        DateTime to = new DateTime(plannedExecutionToLocal.Year, plannedExecutionToLocal.Month, plannedExecutionToLocal.Day, 23, 59, 59);
        DateTime from = new DateTime(plannedExecutionFromLocal.Year, plannedExecutionFromLocal.Month, plannedExecutionFromLocal.Day);

        int days = to.Subtract(from).Days + 1;
        if (command.ShipmentParts.Count != days)
        {
            errors.Add(ShipmentValidationErrors.ShipmentPartsDaysDoesNotMatchText);
        }

        if (errors.Any())
        {
            return Result<UpdateShipmentResult>.Failed(errors);
        }

        ShipmentDetails shipmentDetails = UpdateShipmentCommand.Map(command);
        List<ShipmentPart> shipmentPartsToDelete = await _shipmentPartsRepository.GetByShipmentIdAsync(shipment.Id, cancellationToken);
        _shipmentPartsRepository.Delete(shipmentPartsToDelete);
        List<ShipmentPart> shipmentPartsToAdd = shipment.AddNewShipmentParts(command.ShipmentParts, command.PlannedExecutionFrom.Value, days);
        await _shipmentPartsRepository.InsertManyAsync(shipmentPartsToAdd, cancellationToken);
        shipment.Update(shipmentDetails);
        _shipmentsRepository.Update(shipment);
        //Note: Should we change the status to "Changed" when updating a shipment?
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Shipment updated with id: {ShipmentId}", shipment.Id);
        UpdateShipmentResult updateShipmentResult = UpdateShipmentResult.Map(shipment, shipmentPartsToAdd);
        return Result<UpdateShipmentResult>.Success(updateShipmentResult);
    }
}
