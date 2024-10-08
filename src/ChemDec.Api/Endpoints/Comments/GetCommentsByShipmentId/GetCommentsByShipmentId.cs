﻿using Application.Comments.Queries.GetCommentsByShipmentId;
using Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Comments.GetCommentsByShipmentId;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class GetCommentsByShipmentId : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetCommentsByShipmentId(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{shipmentId}/comments")]
    [SwaggerOperation(Description = "Get comments by shipment",
                               Summary = "Get comments by shipment",
                               Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<GetCommentsByShipmentIdResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId)
    {
        Result<GetCommentsByShipmentIdResult> result = await _queryDispatcher.
            DispatchAsync<GetCommentsByShipmentIdQuery, Result<GetCommentsByShipmentIdResult>>
                        (new GetCommentsByShipmentIdQuery(shipmentId), HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
