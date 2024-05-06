using Application.Attachments.Commands.Delete;
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

namespace ChemDec.Api.Endpoints.Shipments.Attachments.Delete;

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

    [HttpDelete("{shipmentId}/attachments/{attachmentId}")]
    [SwaggerOperation(Description = "Delete attachment in shipment",
                               Summary = "Delete attachment in shipment",
                               Tags = new[] { "Shipments - new" })]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromRoute] Guid attachmentId)
    {
        User user = await _userService.GetUser(User);
        DeleteAttachmentCommand command = new(attachmentId, shipmentId, user.Email, user.Name);
        Result<bool> result = await _commandDispatcher.DispatchAsync<DeleteAttachmentCommand, Result<bool>>(command, HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
