using Application.Chemicals.Commands.DeleteShipmentChemical;
using Application.Common;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Chemicals.Delete;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public sealed class Delete : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;
    public Delete(ICommandDispatcher commandDispatcher, UserService userService)
    {
        _commandDispatcher = commandDispatcher;
        _userService = userService;
    }

    [HttpDelete("{shipmentId}/chemicals/{shipmentChemicalId}")]
    [SwaggerOperation(Description = "Delete chemical from shipment",
                                      Summary = "Delete chemical from shipment. Pass shipment chemical id not chemical id.",
                                        Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromRoute] Guid shipmentChemicalId)
    {
        User user = await _userService.GetUser(User);

        DeleteShipmentChemicalCommand deleteShipmentChemicalCommand = new DeleteShipmentChemicalCommand(shipmentChemicalId, shipmentId, user.Email, user.Name);
        Result<bool> result =
            await _commandDispatcher.DispatchAsync<DeleteShipmentChemicalCommand, Result<bool>>(deleteShipmentChemicalCommand, HttpContext.RequestAborted);

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
