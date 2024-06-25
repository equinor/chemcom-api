using Application.Attachments.Queries.GetAttachmentById;
using Application.Common;
using Application.Shipments.Commands.Update;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Shipments.Attachments.GetById;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class GetById : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;
    public GetById(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{shipmentId}/attachments/{attachmentId}")]
    [SwaggerOperation(Description = "Get shipment attachment by id",
                       Summary = "Get shipment attachment by id",
                       Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromRoute] Guid attachmentId)
    {
        Result<GetAttachmentByIdResult> result = await _queryDispatcher
            .DispatchAsync<GetAttachmentByIdQuery, Result<GetAttachmentByIdResult>>(new GetAttachmentByIdQuery(shipmentId, attachmentId));

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return new FileStreamResult(result.Data.AttchmentStream, result.Data.ContentType)
        {
            FileDownloadName = result.Data.FileName
        };
    }
}
