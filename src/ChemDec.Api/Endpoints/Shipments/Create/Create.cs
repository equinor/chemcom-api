using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Update;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;
using User = Domain.Users.User;

namespace ChemDec.Api.Endpoints.Shipments.Create;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Create : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserProvider _userProvider;

    public Create(ICommandDispatcher commandDispatcher, IUserProvider userProvider)
    {
        _commandDispatcher = commandDispatcher;
        _userProvider = userProvider;
    }

    [HttpPost]
    [SwaggerOperation(Description = "Create new shipment",
                        Summary = "Create new shipment",
                         Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<CreateShipmentResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromBody] CreateShipmentRequest request)
    {
        User user = await _userProvider.GetUserAsync(User);

        CreateShipmentCommand command = new()
        {
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
            ShipmentParts = request.ShipmentParts
        };

        Result<CreateShipmentResult> result = await _commandDispatcher.
            DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(command, HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        string createdAt = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}{HttpContext.Request.Path.ToUriComponent()}/{result.Data.Id}";

        return Created(createdAt, result);
    }
}
