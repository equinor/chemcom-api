using Application.Common;
using Application.Shipments.Queries.GeyShipmentById;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Threading.Tasks;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Application.Chemicals.Queries.GetChemicals;

namespace ChemDec.Api.Endpoints.Chemicals.GetChemicals;

[Route("api/chemicals")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class GetChemicals : ControllerBase
{
    private readonly IQueryDispatcher _queryDispatcher;

    public GetChemicals(IQueryDispatcher queryDispatcher)
    {
        _queryDispatcher = queryDispatcher;
    }

    [HttpGet]
    [SwaggerOperation(Description = "Get chemicals",
                        Summary = "Get chemicals",
                        Tags = new[] { "Chemicals - new" })]
    [ProducesResponseType(typeof(Result<GetChemicalsQueryResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromQuery]GetChemicalsRequest request)
    {
        Result<GetChemicalsQueryResult> result = await _queryDispatcher.
            DispatchAsync<GetChemicalsQuery, Result<GetChemicalsQueryResult>>(new GetChemicalsQuery
                                                                                    (request.ExcludeActive,
                                                                                    request.ExcludeDisabled,
                                                                                    request.ExcludeProposed,
                                                                                    request.ExcludeNotProposed,
                                                                                    request.ForInstallation));

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
