using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Common.Repositories;
using Application.Shipments.Commands;
using Domain.Installations;
using Domain.ShipmentParts;
using Domain.Shipments;
using Domain.Users;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Create;

public sealed class CreateShipmentCommandHandler : ICommandHandler<CreateShipmentCommand, Result<Guid>>
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
        ILogger<CreateShipmentCommandHandler> logger,
        IUserProvider userProvider)
    {
        _shipmentsRepository = shipmentsRepository;
        _installationsRepository = installationsRepository;
        _shipmentPartsRepository = shipmentPartsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    public async Task<Result<Guid>> HandleAsync(CreateShipmentCommand command, CancellationToken cancellationToken = default)
    {
        List<string> errors = new();
        //TODO: Add fluent validation  
        //TODO: Validate receiver id as well?
        //TODO: Should validate volumehasbeenminimizedcomment

        if (command.SenderId == Guid.Empty)
        {
            errors.Add(ShipmentValidationErrors.SenderRequiredText);
        }

        Role role = command.User.Roles.FirstOrDefault(r => r.Installation != null && r.Installation.Id == command.SenderId);
        if (role is null)
        {
            errors.Add(ShipmentValidationErrors.UserAccessForInstallationText);
            return Result<Guid>.Failed(errors);
        }

        if (command.PlannedExecutionFrom is null)
        {
            errors.Add(ShipmentValidationErrors.PlannedExecutionFromDateRequiredText);
        }

        if (command.PlannedExecutionTo is null)
        {
            errors.Add(ShipmentValidationErrors.PlannedExecutionToDateRequiredText);
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
            return Result<Guid>.Failed(errors);
        }

        ShipmentDetails shipmentDetails = CreateShipmentCommand.Map(command);
        Shipment shipment = new(shipmentDetails);

        shipment.SetStatus(Statuses.Draft);
        shipment.SetNewId();
        shipment.SetReceiverId(role.Installation.ShipsTo.Id);
        await _shipmentsRepository.InsertAsync(shipment, cancellationToken);
        List<ShipmentPart> shipmentParts = shipment.AddNewShipmentParts(command.ShipmentParts, command.PlannedExecutionFrom.Value, days);
        await _shipmentPartsRepository.InsertManyAsync(shipmentParts, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Shipment created with id: {ShipmentId}", shipment.Id);
        return Result<Guid>.Success(shipment.Id);
    }
}
