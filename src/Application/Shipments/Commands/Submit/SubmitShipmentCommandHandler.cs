using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Common.Repositories;
using Domain.EmailNotifications;
using Domain.Installations;
using Domain.Shipments;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Submit;

public sealed class SubmitShipmentCommandHandler : ICommandHandler<SubmitShipmentCommand, Result<bool>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IEmailNotificationsRepository _emailNotificationsRepository;
    private IInstallationsRepository _installationsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnvironmentContext _environmentContext;
    private readonly ILogger<SubmitShipmentCommandHandler> _logger;

    //TODO: check if submitting is always from Offshore
    public SubmitShipmentCommandHandler(IShipmentsRepository shipmentsRepository,
        IEmailNotificationsRepository emailNotificationsRepository,
        IUnitOfWork unitOfWork,
        IEnvironmentContext environmentContext,
        ILogger<SubmitShipmentCommandHandler> logger,
        IInstallationsRepository installationsRepository)
    {
        _shipmentsRepository = shipmentsRepository;
        _emailNotificationsRepository = emailNotificationsRepository;
        _unitOfWork = unitOfWork;
        _environmentContext = environmentContext;
        _logger = logger;
        _installationsRepository = installationsRepository;
    }
    public async Task<Result<bool>> HandleAsync(SubmitShipmentCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);
        if (shipment == null)
        {
            return Result<bool>.NotFound([ShipmentValidationErrors.ShipmentNotFoundText]);
        }

        if (shipment.Status != ShipmentStatuses.Draft)
        {
            return Result<bool>.Failed([ShipmentValidationErrors.ShipmentCanNotBeResubmittedText]);
        }

        Installation sender = await _installationsRepository.GetByIdAsync(shipment.SenderId, cancellationToken);
        Installation receiver = await _installationsRepository.GetByIdAsync(shipment.ReceiverId, cancellationToken);

        EmailNotification emailNotification = new()
        {
            Id = Guid.NewGuid(),
            Subject = $"{_environmentContext.GetEnvironmentPrefix()}Shipment form was submitted to {receiver.Name}",
            Body = BuldEmailTemplate(receiver.Name, sender.Name, command.User.Email, command.User.Name),
            Recipients = receiver.Contact,
            EmailNotificationType = (int)EmailNotificationType.EmailNotificationFromOffshore,
            IsSent = false
        };


        if (command.TakePrecaution)
        {
            shipment.Precautions = command.Precautions;
        }

        if (command.HeightenedLra)
        {
            shipment.Pb210 = command.Pb210;
            shipment.Ra226 = command.Ra226;
            shipment.Ra228 = command.Ra228;
        }

        if (command.AvailableForDailyContact.HasValue && command.AvailableForDailyContact.Value is true)
        {
            shipment.NormalProcedure = null;
            shipment.OnlyWayToGetRidOf = null;
        }

        shipment.AvailableForDailyContact = command.AvailableForDailyContact;
        shipment.HeightenedLra = command.HeightenedLra;
        shipment.TakePrecaution = command.TakePrecaution;
        shipment.SetUpdatedInfo(command.User.Email, command.User.Name);
        shipment.SetStatus(ShipmentStatuses.Submitted);

        _shipmentsRepository.Update(shipment);
        await _emailNotificationsRepository.AddAsync(emailNotification, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        _logger.LogInformation("Shipment with id {shipmentId} has been submitted", shipment.Id);
        return Result<bool>.Success(true);
    }


    private string BuldEmailTemplate(string receiver, string destination, string updatedBy, string updatedByName)
    {
        StringBuilder template = new();
        template.AppendLine($"{_environmentContext.GetEnvironmentPrefix()}Shipment form was submitted to {receiver}");
        template.AppendLine($"Submitted by {updatedByName} on {destination} ({updatedBy})");
        return template.ToString();
    }
}
