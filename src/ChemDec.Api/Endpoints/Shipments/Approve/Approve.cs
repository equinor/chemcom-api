using Application.Common;
using Application.Shipments.Commands.Submit;
using ChemDec.Api.Endpoints.Shipments.Submit;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using System;
using Domain.Users;
using Application.Shipments.Commands.Approve;

namespace ChemDec.Api.Endpoints.Shipments.Approve;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Approve : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserProvider _userProvider;
    public Approve(ICommandDispatcher commandDispatcher, IUserProvider userProvider)
    {
        _commandDispatcher = commandDispatcher;
        _userProvider = userProvider;
    }

    [HttpPatch("{shipmentId}/approve")]
    [SwaggerOperation(Description = "Approve a shipment",
                        Summary = "Approve a shipment",
                        Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId)
    {
        User user = await _userProvider.GetUserAsync(User);

        ApproveShipmentCommand approveShipmentCommand = new()
        {
            ShipmentId = shipmentId,
            User = user
        };

        Result<bool> result = await _commandDispatcher.DispatchAsync<ApproveShipmentCommand, Result<bool>>(approveShipmentCommand, HttpContext.RequestAborted);

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
