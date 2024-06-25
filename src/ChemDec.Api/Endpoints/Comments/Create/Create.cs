using Application.Chemicals.Commands.Create;
using Application.Comments.Commands.Create;
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

using User = Domain.Users.User;

namespace ChemDec.Api.Endpoints.Comments.Create;

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

    [HttpPost("{shipmentId}/comments")]
    [SwaggerOperation(Description = "Create new comment",
                               Summary = "Create new comment",
                               Tags = new[] { "Shipments - new" })]
    [ProducesResponseType(typeof(Result<CreateCommentResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromBody] CreateCommentRequest request)
    {
        User user = await _userProvider.GetUserAsync(User);
        CreateCommentCommand command = new CreateCommentCommand(request.CommentText, shipmentId, user);
        Result<CreateCommentResult> result = await _commandDispatcher.DispatchAsync<CreateCommentCommand, Result<CreateCommentResult>>(command, HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        string createdAt = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}{HttpContext.Request.Path.ToUriComponent()}/{result.Data.Id}";
        return Created(createdAt, result);
    }
}
