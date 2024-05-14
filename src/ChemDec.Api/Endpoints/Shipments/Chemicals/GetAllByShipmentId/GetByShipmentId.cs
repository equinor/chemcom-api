using Application.Chemicals.Queries.GetShipmentChemicalsByShipmentIdCommand;
using Application.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Chemicals.GetByShipmentId;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public sealed class GetByShipmentId : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetByShipmentId(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet("{shipmentId}/chemicals")]
    [SwaggerOperation(Description = "Get shipment chemicals",
                               Summary = "Get shipment chemicals",
                               Tags = new[] { "Shipments - new" })]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId)
    {
        Result<GetShipmentChemicalsByShipmentIdResult> result = await _queryDispatcher
            .DispatchAsync<GetShipmentChemicalsByShipmentIdQuery, Result<GetShipmentChemicalsByShipmentIdResult>>(new GetShipmentChemicalsByShipmentIdQuery(shipmentId));

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
