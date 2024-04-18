using Application.Common;
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

    public async Task<Result<UpdateShipmentResult>> HandleAsync(UpdateShipmentCommand command)
    {
        List<string> errors = new();
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.Id);

        if (shipment is null)
        {
            errors.Add("Shipment not found");
            return Result<UpdateShipmentResult>.NotFound(errors);
        }
        //TODO: Add fluent validation  
        if (command.SenderId == Guid.Empty)
        {
            errors.Add("Sender is required");
        }

        if (command.PlannedExecutionFrom is null || command.PlannedExecutionTo is null)
        {
            errors.Add("Planned execution from date is required");
        }

        if (command.PlannedExecutionTo is null)
        {
            errors.Add("Planned execution to date is required");
        }

        if (command.Initiator == Initiator.Offshore && !command.IsInstallationPartOfUserRoles)
        {
            errors.Add("User do not have access to save from this installation");
        }

        Installation installation = await _installationsRepository.GetByIdAsync(command.SenderId);
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(installation.TimeZone);
        DateTime plannedExecutionFrom = TimeZoneInfo.ConvertTimeFromUtc(command.PlannedExecutionFrom.Value, timeZone);
        DateTime plannedExecutionTo = TimeZoneInfo.ConvertTimeFromUtc(command.PlannedExecutionTo.Value, timeZone);

        int days = plannedExecutionTo.Subtract(plannedExecutionFrom).Days + 1;
        int shipmentPartsCount = command.ShipmentParts.Count();
        if (shipmentPartsCount != days)
        {
            errors.Add("Days does not match the execution dates. This should normally not happen");
        }

        if (errors.Any())
        {
            return Result<UpdateShipmentResult>.Failed(errors);
        }

        ShipmentDetails shipmentDetails = UpdateShipmentCommand.Map(command);
        List<ShipmentPart> shipmentPartsToDelete = await _shipmentPartsRepository.GetByShipmentIdAsync(shipment.Id);
        _shipmentPartsRepository.Delete(shipmentPartsToDelete);
        List<ShipmentPart> shipmentPartsToAdd = shipment.AddNewShipmentParts(command.ShipmentParts.Select(shipmentPart => (int)shipmentPart.Value).ToList(), plannedExecutionFrom, days);
        await _shipmentPartsRepository.InsertManyAsync(shipmentPartsToAdd);
        shipment.Update(shipmentDetails);
        //Note: Should we change the status to "Changed" when updating a shipment?
        await _unitOfWork.CommitChangesAsync();
        UpdateShipmentResult updateShipmentResult = UpdateShipmentResult.Map(shipment);
        return Result<UpdateShipmentResult>.Success(updateShipmentResult);
    }
}
