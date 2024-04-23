using Application.Attachments.Commands.Create;
using Application.Common;
using Azure.Core;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.IO;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Shipments.Attachments.Create;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public sealed class Create : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;

    public Create(ICommandDispatcher commandDispatcher, UserService userService)
    {
        _commandDispatcher = commandDispatcher;
        _userService = userService;
    }

    [HttpPost("{shipmentId}/attachments")]
    [SwaggerOperation(Description = "Create attachment in shipment",
                        Summary = "Create new attachment in shipment",
                        Tags = new[] { "Shipments - new" })]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromForm] IFormFile attachment)
    {
        User user = await _userService.GetUser(User);

        using (MemoryStream stream = new MemoryStream())
        {
            await attachment.CopyToAsync(stream);
            var fileContents = stream.ToArray();
            string extension = Path.GetExtension(attachment.FileName);
            CreateAttachmentCommand command = new(shipmentId, attachment.FileName, attachment.ContentType, extension, fileContents, user.Email, user.Name);
            Result<bool> result = await _commandDispatcher.DispatchAsync<CreateAttachmentCommand, Result<bool>>(command);

            if (result.Status == ResultStatusConstants.NotFound)
            {
                return NotFound(result);
            }

            return Created(string.Empty, result);
        }
    }
}
