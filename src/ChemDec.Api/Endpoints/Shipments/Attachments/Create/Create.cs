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
using User = Domain.Users.User;
namespace ChemDec.Api.Endpoints.Shipments.Attachments.Create;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public sealed class Create : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserProvider _userProvider;

    public Create(ICommandDispatcher commandDispatcher, IUserProvider userProvider)
    {
        _commandDispatcher = commandDispatcher;
        _userProvider = userProvider;
    }

    [HttpPost("{shipmentId}/attachments")]
    [SwaggerOperation(Description = "Create attachment in shipment",
                        Summary = "Create new attachment in shipment",
                          Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, IFormFile attachment)
    {
        User user = await _userProvider.GetUserAsync(User);

        using (MemoryStream stream = new MemoryStream())
        {
            await attachment.CopyToAsync(stream);
            var fileContents = stream.ToArray();
            string extension = Path.GetExtension(attachment.FileName);
            CreateAttachmentCommand command = new(shipmentId, attachment.FileName, extension, attachment.ContentType, fileContents, user);
            Result<CreateAttachmentResult> result = await _commandDispatcher.DispatchAsync<CreateAttachmentCommand, Result<CreateAttachmentResult>>(command, HttpContext.RequestAborted);

            if (result.Status == ResultStatusConstants.NotFound)
            {
                return NotFound(result);
            }

            return Created(string.Empty, result);
        }
    }
}
