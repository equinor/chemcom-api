using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Common.Repositories;
using Application.Shipments.Commands.Create;
using Domain.Installations;
using Domain.ShipmentParts;
using Domain.Shipments;
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
    public UpdateShipmentCommandHandler(IShipmentsRepository shipmentsRepository,
         IInstallationsRepository installationsRepository,
         IShipmentPartsRepository shipmentPartsRepository,
         IUnitOfWork unitOfWork)
    {
        _shipmentsRepository = shipmentsRepository;
        _installationsRepository = installationsRepository;
        _shipmentPartsRepository = shipmentPartsRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Result<UpdateShipmentResult>> HandleAsync(UpdateShipmentCommand command, CancellationToken cancellationToken = default)
    {
        List<string> errors = new();
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (shipment is null)
        {
            errors.Add(ShipmentValidationErrors.ShipmentNotFoundText);
            return Result<UpdateShipmentResult>.NotFound(errors);
        }
        //TODO: Add fluent validation  
        if (command.SenderId == Guid.Empty)
        {
            errors.Add(ShipmentValidationErrors.SenderRequiredText);
        }

        if (command.PlannedExecutionFrom is null)
        {
            errors.Add(ShipmentValidationErrors.PlannedExecutionFromDateRequiredText);
        }

        if (command.PlannedExecutionTo is null)
        {
            errors.Add(ShipmentValidationErrors.PlannedExecutionToDateRequiredText);
        }

        if (command.Initiator == Initiator.Offshore && !command.IsInstallationPartOfUserRoles)
        {
            errors.Add(ShipmentValidationErrors.UserAccessForInstallationText);
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
        UpdateShipmentResult updateShipmentResult = UpdateShipmentResult.Map(shipment);
        return Result<UpdateShipmentResult>.Success(updateShipmentResult);
    }
}
