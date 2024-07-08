using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Application.Shipments.Commands.Update;
using Domain.Shipments;
using Domain.Users;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Shipments.Commands.Evaluate;

public sealed class SaveEvaluationCommandHandler : ICommandHandler<SaveEvaluationCommand, Result<bool>>
{
    private readonly IShipmentsRepository _shipmentsRepository;
    private readonly IInstallationsRepository _installationsRepository;
    private readonly IEmailNotificationsRepository _emailNotificationsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<SaveEvaluationCommandHandler> _logger;

    public SaveEvaluationCommandHandler(
        IShipmentsRepository shipmentsRepository,
        IInstallationsRepository installationsRepository,
        IEmailNotificationsRepository emailNotificationsRepository,
        IUnitOfWork unitOfWork,
        ILogger<SaveEvaluationCommandHandler> logger)
    {
        _shipmentsRepository = shipmentsRepository;
        _installationsRepository = installationsRepository;
        _emailNotificationsRepository = emailNotificationsRepository;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }
    //TODO: Get the recent version of the shipment
    //TODO: Send email
    public async Task<Result<bool>> HandleAsync(SaveEvaluationCommand command, CancellationToken cancellationToken = default)
    {
        Shipment shipment = await _shipmentsRepository.GetByIdAsync(command.ShipmentId, cancellationToken);
        if (shipment is null)
        {
            return Result<bool>.NotFound([ShipmentValidationErrors.ShipmentNotFoundText]);
        }

        Role receiverRole = command.User.Roles.FirstOrDefault(r => r.Installation.Id == shipment.ReceiverId);
        if (receiverRole is null)
        {
            return Result<bool>.NotFound([ShipmentValidationErrors.ShipmentNoAccessUserToEvaluateText]);
        }

        shipment.UpdateEvaluationValues(command.EvalAmountOk,
            command.EvalBiocidesOk,
            command.EvalCapacityOk,
            command.EvalContaminationRisk,
            command.EvalEnvImpact,
            command.EvalComments,
            command.User);

        _shipmentsRepository.Update(shipment);
        await _unitOfWork.CommitChangesAsync();
        _logger.LogInformation("Evaluation saved for shipment with id: {ShipmentId}", shipment.Id);
        return Result<bool>.Success(true);
    }
}
