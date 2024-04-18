using Application.Common;
using Application.Shipments.Queries;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Shipments.GetById;

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

    [HttpGet]
    [Route("{id}")]
    [SwaggerOperation(Description = "Get shipment",
                        Summary = "Get shipment",
                        Tags = new[] { "Shipments" })]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid id)
    {
        Result<GetShipmentByIdQueryResult> result = await _queryDispatcher.DispatchAsync<GetShipmentByIdQuery, Result<GetShipmentByIdQueryResult>>(new GetShipmentByIdQuery(id));

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
