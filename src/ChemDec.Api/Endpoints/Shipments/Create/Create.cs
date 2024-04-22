using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Update;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Shipments.Create;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
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
    [SwaggerOperation(Description = "Create new shipment",
                        Summary = "Create new shipment",
                        Tags = new[] { "Shipments - new" })]
    [ProducesResponseType(typeof(Result<CreateShipmentResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
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

        CreateShipmentCommand command = new()
        {
            SenderId = request.SenderId,
            Code = request.Code,
            ReceiverId = receiverId,
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
            IsInstallationPartOfUserRoles = isInstallationPartOfUserRoles,
            ShipmentParts = request.ShipmentParts,
            UpdatedBy = user.Email,
            UpdatedByName = user.Name,
        };

        Result<CreateShipmentResult> result = await _commandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(command);

        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        Uri createdAt = new($"{HttpContext.Request.Host}/api/shipments/{result.Data.Id}");
        return Created(createdAt, result);
    }
}
