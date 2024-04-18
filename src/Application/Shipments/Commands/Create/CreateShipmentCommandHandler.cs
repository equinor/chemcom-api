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

public sealed class CreateShipmentCommandHandler : ICommandHandler<CreateShipmentCommand, Result<CreateShipmentResult>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IInstallationsRepository _installationsRepository;
    private readonly IShipmentPartsRepository _shipmentPartsRepository;
    private readonly IUnitOfWork _unitOfWork;

    //TODO: Add logging

    public CreateShipmentCommandHandler(IShipmentsRepository shipmentsRepository,
        IInstallationsRepository installationsRepository,
        IShipmentPartsRepository shipmentPartsRepository,
        IUnitOfWork unitOfWork)
    {
        _shipmentsRepository = shipmentsRepository;
        _installationsRepository = installationsRepository;
        _shipmentPartsRepository = shipmentPartsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<CreateShipmentResult>> HandleAsync(CreateShipmentCommand command)
    {
        List<string> errors = new();
        //TODO: Add fluent validation  
        //TODO: Validate receiver id as well?
        //TODO: Should validate volumehasbeenminimizedcomment
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

        //if (string.IsNullOrWhiteSpace(command.VolumeHasBeenMinimizedComment))
        //{
        //    result.Errors.Add("Missing specification on measures taken to minimize well fluids / water volume sent to land");
        //}

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
            return Result<CreateShipmentResult>.Failed(errors);
        }

        ShipmentDetails shipmentDetails = CreateShipmentCommand.Map(command);
        Shipment shipment = new Shipment(shipmentDetails);
        shipment.SetStatus(Statuses.Draft);
        shipment.SetNewId();
        await _shipmentsRepository.InsertAsync(shipment);
        List<ShipmentPart> shipmentParts = shipment.AddNewShipmentParts(command.ShipmentParts, plannedExecutionFrom, days);
        await _shipmentPartsRepository.InsertManyAsync(shipmentParts);
        await _unitOfWork.CommitChangesAsync();


        CreateShipmentResult createShipmentResult = CreateShipmentResult.Map(shipment, shipmentParts);
        return Result<CreateShipmentResult>.Success(createShipmentResult);
    }
}
