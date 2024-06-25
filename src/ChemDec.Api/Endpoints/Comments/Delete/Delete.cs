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
using ChemDec.Api.Model;
using User = Domain.Users.User;
namespace ChemDec.Api.Endpoints.Comments.Delete;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Delete : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserProvider _userProvider;

    public Delete(ICommandDispatcher commandDispatcher, IUserProvider userProvider)
    {
        _commandDispatcher = commandDispatcher;
        _userProvider = userProvider;
    }

    [HttpDelete("{shipmentId}/comments/{id}")]
    [SwaggerOperation(Description = "Delete comment",
                               Summary = "Delete comment",
                               Tags = ["Shipments - new" ])]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromRoute] Guid id)
    {
        User user = await _userProvider.GetUserAsync(User);

        Result<bool> result = await _commandDispatcher.DispatchAsync<DeleteCommentCommand, Result<bool>>(new DeleteCommentCommand(id, shipmentId, user), HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
