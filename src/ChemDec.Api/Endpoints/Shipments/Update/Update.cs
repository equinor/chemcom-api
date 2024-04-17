using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Update;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Shipments.Update;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Update : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;

    public Update(ICommandDispatcher commandDispatcher, UserService userService)
    {
        _commandDispatcher = commandDispatcher;
        _userService = userService;
    }

    [HttpPut]
    [Route("{shipmentId}")]
    [SwaggerOperation(Description = "Create new shipment",
                        Summary = "Create new shipment",
                        Tags = new[] { "Shipments" })]
    public async Task<IActionResult> HandleAsync(Guid id, [FromBody] UpdateShipmentRequest request)
    {
        if (Enum.TryParse(request.Initiator, out Initiator initiator) is false)
        {
            return BadRequest("Invalid initiator");
        }

        User user = await _userService.GetUser(User);
        bool isInstallationPartOfUserRoles = false;
        Guid receiverId = Guid.Empty;
        var role = user.Roles.FirstOrDefault(i => i.Installation.Id == request.SenderId);

        if (role != null)
        {
            isInstallationPartOfUserRoles = true;
            receiverId = role.Installation.Id;
        }

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
            IsInstallationPartOfUserRoles = isInstallationPartOfUserRoles
        };

        Result<UpdateShipmentResult> result = await _commandDispatcher.DispatchAsync<UpdateShipmentCommand, Result<UpdateShipmentResult>>(command);

        if (result.Errors.Any())
        {
            return BadRequest(result);
        }
        return Ok(result);
    }
}
