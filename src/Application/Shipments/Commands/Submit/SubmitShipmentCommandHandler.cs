using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Common.Repositories;
using Domain.EmailNotifications;
using Domain.Installations;
using Domain.Shipments;
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
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEnvironmentContext _environmentContext;

    //TODO: check if submitting is always from Offshore
    public SubmitShipmentCommandHandler(IShipmentsRepository shipmentsRepository,
        IEmailNotificationsRepository emailNotificationsRepository,
        IUnitOfWork unitOfWork,
        IEnvironmentContext environmentContext)
    {
        _shipmentsRepository = shipmentsRepository;
        _emailNotificationsRepository = emailNotificationsRepository;
        _unitOfWork = unitOfWork;
        _environmentContext = environmentContext;
    }
    public async Task<Result<bool>> HandleAsync(SubmitShipmentCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);
        if (shipment == null)
        {
            return Result<bool>.NotFound(new List<string> { "Shipment not found" });
        }

        if (shipment.Status != ShipmentStatuses.Draft)
        {
            return Result<bool>.Failed(new List<string> { "Can't re-submit shipment" });
        }

        EmailNotification emailNotification = new()
        {
            Id = Guid.NewGuid(),
            Subject = $"{_environmentContext.GetEnvironmentPrefix()}Shipment form was submitted to {shipment.Receiver.Name}",
            Body = BuldEmailTemplate(shipment.Receiver.Name, shipment.Sender.Name, command.UpdatedBy, command.UpdatedByName),
            Recipients = shipment.Receiver.Contact,
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

        if (command.AvailableForDailyContact.Value)
        {
            shipment.NormalProcedure = null;
            shipment.OnlyWayToGetRidOf = null;
        }

        shipment.AvailableForDailyContact = command.AvailableForDailyContact;
        shipment.HeightenedLra = command.HeightenedLra;
        shipment.TakePrecaution = command.TakePrecaution;
        shipment.Updated = DateTime.Now;
        shipment.UpdatedBy = command.UpdatedBy;
        shipment.UpdatedByName = command.UpdatedByName;
        shipment.Status = ShipmentStatuses.Submitted;

        _shipmentsRepository.Update(shipment);
        await _emailNotificationsRepository.AddAsync(emailNotification, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);

        return Result<bool>.Success(true);
    }


    private string BuldEmailTemplate(string receiver, string destination, string updatedBy, string updatedByName)
    {
        StringBuilder template = new();
        template.AppendLine($"{_environmentContext.GetEnvironmentPrefix}Shipment form was submitted to {receiver}");
        template.AppendLine($"Submitted by {updatedByName} on {destination} ({updatedBy})");
        return template.ToString();
    }
}
