using Application.Attachments.Queries.GetAttachmentsByShipmentId;
using Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Shipments.Attachments.GetAttachmentsByShipmentId;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class GetAttachmentsByShipmentId : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetAttachmentsByShipmentId(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{shipmentId}/attachments")]
    [SwaggerOperation(Description = "Get all attachments by shipment",
                               Summary = "Get all attachments by shipment",
                                 Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<GetAttachmentsByShipmentIdResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId)
    {
        GetAttachmentsByShipmentIdQuery query = new(shipmentId);
        Result<GetAttachmentsByShipmentIdResult> result = 
            await _queryDispatcher.DispatchAsync<GetAttachmentsByShipmentIdQuery, Result<GetAttachmentsByShipmentIdResult>>(query, HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
