﻿using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Update;
using ChemDec.Api.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Net.Mime;
using System.Threading.Tasks;

using User = Domain.Users.User;

namespace ChemDec.Api.Endpoints.Shipments.Update;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Update : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserProvider _userProvider;
    public Update(ICommandDispatcher commandDispatcher, IUserProvider userProvider)
    {
        _commandDispatcher = commandDispatcher;
        _userProvider = userProvider;
    }

    [HttpPut]
    [Route("{id}")]
    [SwaggerOperation(Description = "Update shipment",
                        Summary = "Update shipment",
                        Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid id, [FromBody] UpdateShipmentRequest request)
    {
        User user = await _userProvider.GetUserAsync(User);

        UpdateShipmentCommand command = new()
        {
            Id = id,
            SenderId = request.SenderId,
            Code = request.Code,
            Title = request.Title,
            Type = request.Type,
            PlannedExecutionFrom = request.PlannedExecutionFrom.Value,
            PlannedExecutionTo = request.PlannedExecutionTo.Value,
            WaterAmount = request.WaterAmount,
            WaterAmountPerHour = request.WaterAmountPerHour,
            Well = request.Well,
            VolumePersentageOffspec = request.VolumePersentageOffspec,
            ContainsChemicals = request.ContainsChemicals,
            ContainsStableOilEmulsion = request.ContainsStableOilEmulsion,
            ContainsHighParticleAmount = request.ContainsHighParticleAmount,
            ContainsBiocides = request.ContainsBiocides,
            VolumeHasBeenMinimized = request.VolumeHasBeenMinimized,
            VolumeHasBeenMinimizedComment = request.VolumeHasBeenMinimizedComment,
            NormalProcedure = request.NormalProcedure,
            OnlyWayToGetRidOf = request.OnlyWayToGetRidOf,
            OnlyWayToGetRidOfComment = request.OnlyWayToGetRidOfComment,
            AvailableForDailyContact = request.AvailableForDailyContact,
            HeightenedLra = request.HeightenedLra,
            Pb210 = request.Pb210,
            Ra226 = request.Ra226,
            Ra228 = request.Ra228,
            TakePrecaution = request.TakePrecaution,
            Precautions = request.Precautions,
            WaterHasBeenAnalyzed = request.WaterHasBeenAnalyzed,
            HasBeenOpened = request.HasBeenOpened,
            RinsingOffshorePercent = request.RinsingOffshorePercent,
            ShipmentParts = request.ShipmentParts,
            User = user
        };

        Result<Guid> result = await _commandDispatcher
            .DispatchAsync<UpdateShipmentCommand, Result<Guid>>(command, HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
