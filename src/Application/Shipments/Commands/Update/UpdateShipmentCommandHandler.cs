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
        Result<UpdateShipmentResult> result = new();
        //TODO: Add fluent validation  
        if (command.SenderId == Guid.Empty)
        {
            result.Errors.Add("Sender is required");
        }

        if (command.PlannedExecutionFrom is null || command.PlannedExecutionTo is null)
        {
            result.Errors.Add("Planned execution from date is required");
        }

        if (command.PlannedExecutionTo is null)
        {
            result.Errors.Add("Planned execution to date is required");
        }

        if (command.Initiator == Initiator.Offshore && !command.IsInstallationPartOfUserRoles)
        {
            result.Errors.Add("User do not have access to save from this installation");
        }

        Installation installation = await _installationsRepository.GetByIdAsync(command.SenderId);
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(installation.TimeZone);
        DateTime plannedExecutionFrom = TimeZoneInfo.ConvertTimeFromUtc(command.PlannedExecutionFrom.Value, timeZone);
        DateTime plannedExecutionTo = TimeZoneInfo.ConvertTimeFromUtc(command.PlannedExecutionTo.Value, timeZone);

        int days = plannedExecutionTo.Subtract(plannedExecutionFrom).Days + 1;
        int shipmentPartsCount = command.ShipmentParts.Count();
        if (shipmentPartsCount != days)
        {
            result.Errors.Add("Days does not match the execution dates. This should normally not happen");
        }

        if (result.Errors.Any())
        {
            return result;
        }

        ShipmentDetails shipmentDetails = UpdateShipmentCommand.Map(command);
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.Id);

        if (shipment is null)
        {
            result.Errors.Add("Shipment not found");
            result.Status = ResultStatusConstants.NotFound;
            return result;
        }

        //TODO: fix concurrency issues

        //List<ShipmentPart> shipmentPartsToDelete = await _shipmentPartsRepository.GetByShipmentId(shipment.Id);
        //_shipmentPartsRepository.Delete(shipmentPartsToDelete);
        //await _unitOfWork.CommitChangesAsync();
        shipment.ShipmentParts.Clear();
        //shipment.AddNewShipmentParts(command.ShipmentParts.Select(shipmentPart => (int)shipmentPart.Value).ToList(), plannedExecutionFrom, days);

        shipment.Update(shipmentDetails);
        //Note: Should we change the status to "Changed" when updating a shipment?
        await _unitOfWork.CommitChangesAsync();
        UpdateShipmentResult updateShipmentResult = UpdateShipmentResult.Map(shipment);
        result.Status = ResultStatusConstants.Success;
        result.Data = updateShipmentResult;
        return result;
    }
}
