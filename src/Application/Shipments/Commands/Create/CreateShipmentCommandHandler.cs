using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Common.Repositories;
using Application.Shipments.Commands;
using Domain.Installations;
using Domain.ShipmentParts;
using Domain.Shipments;
using Microsoft.Extensions.Logging;
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
    private readonly ILogger<CreateShipmentCommandHandler> _logger;

    public CreateShipmentCommandHandler(IShipmentsRepository shipmentsRepository,
        IInstallationsRepository installationsRepository,
        IShipmentPartsRepository shipmentPartsRepository,
        IUnitOfWork unitOfWork,
        ILogger<CreateShipmentCommandHandler> logger)
    {
        _shipmentsRepository = shipmentsRepository;
        _installationsRepository = installationsRepository;
        _shipmentPartsRepository = shipmentPartsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result<CreateShipmentResult>> HandleAsync(CreateShipmentCommand command, CancellationToken cancellationToken = default)
    {
        List<string> errors = new();
        //TODO: Add fluent validation  
        //TODO: Validate receiver id as well?
        //TODO: Should validate volumehasbeenminimizedcomment
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
            return Result<CreateShipmentResult>.Failed(errors);
        }

        //if (string.IsNullOrWhiteSpace(command.VolumeHasBeenMinimizedComment))
        //{
        //    result.Errors.Add("Missing specification on measures taken to minimize well fluids / water volume sent to land");
        //}

        Installation installation = await _installationsRepository.GetByIdAsync(command.SenderId, cancellationToken);
        TimeZoneInfo timeZone = TimeZoneInfo.FindSystemTimeZoneById(installation.TimeZone);
        DateTime plannedExecutionFrom = TimeZoneInfo.ConvertTimeFromUtc(command.PlannedExecutionFrom.Value, timeZone);
        DateTime plannedExecutionTo = TimeZoneInfo.ConvertTimeFromUtc(command.PlannedExecutionTo.Value, timeZone);

        int days = plannedExecutionTo.Subtract(plannedExecutionFrom).Days + 1;
        int shipmentPartsCount = command.ShipmentParts.Count;
        if (shipmentPartsCount != days)
        {
            errors.Add(ShipmentValidationErrors.ShipmentPartsDaysDoesNotMatchText);
        }

        if (errors.Any())
        {
            return Result<CreateShipmentResult>.Failed(errors);
        }

        ShipmentDetails shipmentDetails = CreateShipmentCommand.Map(command);
        Shipment shipment = new(shipmentDetails);
        shipment.SetStatus(Statuses.Draft);
        shipment.SetNewId();
        await _shipmentsRepository.InsertAsync(shipment, cancellationToken);
        List<ShipmentPart> shipmentParts = shipment.AddNewShipmentParts(command.ShipmentParts, plannedExecutionFrom, days);
        await _shipmentPartsRepository.InsertManyAsync(shipmentParts, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Shipment created with id: {ShipmentId}", shipment.Id);

        CreateShipmentResult createShipmentResult = CreateShipmentResult.Map(shipment, shipmentParts);
        return Result<CreateShipmentResult>.Success(createShipmentResult);
    }
}
