using Application.Comments.Commands.Create;
using Application.Common;
using ChemDec.Api.Endpoints.Comments.Create;
using ChemDec.Api.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using System;
using Application.Comments.Commands.Delete;

namespace ChemDec.Api.Endpoints.Comments.Delete;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Delete : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;

    public Delete(ICommandDispatcher commandDispatcher)
    {
        _commandDispatcher = commandDispatcher;
    }

    [HttpDelete("{shipmentId}/comments/{id}")]
    [SwaggerOperation(Description = "Delete comment",
                               Summary = "Delete comment",
                               Tags = new[] { "Shipments - new" })]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromRoute] Guid id)
    {
        Result<bool> result = await _commandDispatcher.DispatchAsync<DeleteCommentCommand, Result<bool>>(new DeleteCommentCommand(id, shipmentId));

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
