using Application.Common;
using Application.Common.Enums;
using Application.Common.Repositories;
using Application.Shipments.Commands;
using Domain.Installations;
using Domain.ShipmentParts;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Create;

public sealed class CreateShipmentCommandHandler : ICommandHandler<CreateShipmentCommand, Result>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IInstallationsRepository _installationsRepository;
    private readonly IUnitOfWork _unitOfWork;

    //TODO: Add logging

    public CreateShipmentCommandHandler(IShipmentsRepository shipmentsRepository,        
        IInstallationsRepository installationsRepository,
        IUnitOfWork unitOfWork)
    {
        _shipmentsRepository = shipmentsRepository;
        _installationsRepository = installationsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result> HandleAsync(CreateShipmentCommand command)
    {
        Result result = new();       
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

        ShipmentDetails shipmentDetails = CreateShipmentCommand.Map(command);
        Shipment shipment = new Shipment(shipmentDetails);
        shipment.SetStatus(Statuses.Draft);
        shipment.SetNewId();
        shipment.AddNewShipmentParts(command.ShipmentParts, plannedExecutionFrom, days);
        await _shipmentsRepository.InsertAsync(shipment);
        await _unitOfWork.CommitChangesAsync();

        //TODO: Do I need to fetch the whole shipment object? May be have to do it in update command handler
        result.Data = shipment;
        return result;
    }
}
