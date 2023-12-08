﻿using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Shipments;

[Route("api/shipments")]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize]
public class Create : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;

    public Create(ICommandDispatcher commandDispatcher, UserService userService)
    {
        _commandDispatcher = commandDispatcher;
        _userService = userService;
    }

    [HttpPost]
    public async Task<IActionResult> HandleAsync([FromBody] CreateShipmentRequest request)
    {
        if (Enum.TryParse(request.Initiator, out Initiator initiator) is false)
        {
            return BadRequest("Invalid initiator");
        }

        User user = await _userService.GetUser(User);
        bool isInstallationPartOfUserRoles = false;
        Guid receiverId = Guid.Empty;
        var role = user.Roles.FirstOrDefault(i => i.Installation.Id == request.SenderId);

        if (role is not null)
        {
            isInstallationPartOfUserRoles = true;
            receiverId = role.Installation.ShipsTo.FirstOrDefault().Id;
        }

        CreateShipmentCommand command = new CreateShipmentCommand()
        {
            Code = request.Code,
            Title = request.Title,
            SenderId = request.SenderId,
            ReceiverId = receiverId,
            Type = request.Type,
            Initiator = initiator,
            IsInstallationPartOfUserRoles = isInstallationPartOfUserRoles,
            PlannedExecutionFrom = request.PlannedExecutionFrom,
            PlannedExecutionTo = request.PlannedExecutionTo,
            WaterAmount = request.WaterAmount,
            WaterAmountPerHour = request.WaterAmountPerHour,
            Well = request.Well,
            ShipmentParts = request.ShipmentParts,
            UpdatedBy = user.Email,
            UpdatedByName = user.Name
        };

        bool result = await _commandDispatcher.DispatchAsync<CreateShipmentCommand, bool>(command);
        return Ok(result);
    }
}
