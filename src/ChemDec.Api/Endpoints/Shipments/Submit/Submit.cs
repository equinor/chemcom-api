using Application.Common;
using Application.Shipments.Commands.Submit;
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

namespace ChemDec.Api.Endpoints.Shipments.Submit;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Submit : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;
    public Submit(ICommandDispatcher commandDispatcher, UserService userService)
    {
        _commandDispatcher = commandDispatcher;
        _userService = userService;
    }

    [HttpPatch("{shipmentId}/submit")]
    [SwaggerOperation(Description = "Create new shipment",
                        Summary = "Create new shipment",
                        Tags = new[] { "Shipments - new" })]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromBody] SubmitShipmentRequest request)
    {
        User user = await _userService.GetUser(User);

        //Guid receiverId = Guid.Empty;
        //bool doesUserHasAccessToTheShipment = false;
        //var role = user.Roles.FirstOrDefault(i => i.Id == request.SenderId.ToString());

        //if (role is not null)
        //{
        //    receiverId = role.Installation.ShipsTo.FirstOrDefault().Id;
        //    var receiverRole = user.Roles.FirstOrDefault(r => r.Id == receiverId.ToString());
        //    if(receiverRole is not null)
        //    {
        //        doesUserHasAccessToTheShipment = true;
        //    }
        //}

        SubmitShipmentCommand command = new()
        {
            ShipmentId = shipmentId,
            AvailableForDailyContact = request.AvailableForDailyContact,
            HeightenedLra = request.HeightenedLra,
            TakePrecaution = request.TakePrecaution,
            Precautions = request.Precautions,
            Pb210 = request.Pb210,
            Ra226 = request.Ra226,
            Ra228 = request.Ra228,
            UpdatedBy = user.Email,
            UpdatedByName = user.Name
        };

        Result<bool> result = await _commandDispatcher.DispatchAsync<SubmitShipmentCommand, Result<bool>>(command, HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
