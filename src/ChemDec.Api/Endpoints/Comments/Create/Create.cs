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

namespace ChemDec.Api.Endpoints.Comments.Create;

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

    [HttpPost("{shipmentId}/comments")]
    [SwaggerOperation(Description = "Create new comment",
                               Summary = "Create new comment",
                               Tags = new[] { "Shipments - new" })]
    [ProducesResponseType(typeof(Result<CreateCommentResult>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromBody] CreateCommentRequest request)
    {
        User user = await _userService.GetUser(User);
        CreateCommentCommand command = new CreateCommentCommand(request.CommentText, shipmentId, user.Email, user.Name);
        Result<CreateCommentResult> result = await _commandDispatcher.DispatchAsync<CreateCommentCommand, Result<CreateCommentResult>>(command);

        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        Uri createdAt = new Uri($"{HttpContext.Request.Host}/api/shipments/{shipmentId}/comments/{result.Data.Id}");

        return Created(createdAt, result);
    }
}
