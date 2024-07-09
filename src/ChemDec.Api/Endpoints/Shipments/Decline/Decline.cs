using Application.Common;
using Application.Shipments.Commands.Approve;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using System;
using Domain.Users;
using Application.Shipments.Commands.Decline;

namespace ChemDec.Api.Endpoints.Shipments.Decline;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Decline : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserProvider _userProvider;
    public Decline(ICommandDispatcher commandDispatcher, IUserProvider userProvider)
    {
        _commandDispatcher = commandDispatcher;
        _userProvider = userProvider;
    }

    [HttpPatch("{shipmentId}/decline")]
    [SwaggerOperation(Description = "Decline a shipment",
                        Summary = "Decline a shipment",
                        Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId)
    {
        User user = await _userProvider.GetUserAsync(User);

        DeclineShipmentCommand approveShipmentCommand = new()
        {
            ShipmentId = shipmentId,
            User = user
        };

        Result<bool> result = await _commandDispatcher.DispatchAsync<DeclineShipmentCommand, Result<bool>>(approveShipmentCommand, HttpContext.RequestAborted);

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

