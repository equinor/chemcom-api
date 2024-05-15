using Application.Chemicals.Commands.UpdateShipmentChemical;
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

namespace ChemDec.Api.Endpoints.Shipments.Chemicals.Update;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Update : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;
    public Update(ICommandDispatcher commandDispatcher, UserService userService)
    {
        _commandDispatcher = commandDispatcher;
        _userService = userService;
    }

    [HttpPut("{shipmentId}/chemicals/{chemicalId}")]
    [SwaggerOperation(Description = "Update chemical in shipment",
                              Summary = "Update chemical in shipment",
                              Tags = new[] { "Shipments - new" })]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromRoute] Guid chemicalId, UpdateShipmentChemicalRequest request)
    {
        User user = await _userService.GetUser(User);
        UpdateShipmentChemicalCommand command = new()
        {
            ShipmentId = shipmentId,
            ChemicalId = chemicalId,
            MeasureUnit = request.MeasureUnit,
            Amount = request.Amount,
            CalculatedWeightUnrinsed = request.CalculatedWeightUnrinsed,
            CalculatedTocUnrinsed = request.CalculatedTocUnrinsed,
            CalculatedNitrogenUnrinsed = request.CalculatedNitrogenUnrinsed,
            CalculatedBiocidesUnrinsed = request.CalculatedBiocidesUnrinsed,
            CalculatedWeight = request.CalculatedWeight,
            CalculatedToc = request.CalculatedToc,
            CalculatedNitrogen = request.CalculatedNitrogen,
            CalculatedBiocides = request.CalculatedBiocides,
            UpdatedByName = user.Name,
            UpdatedBy = user.Email
        };

        Result<bool> result = await _commandDispatcher.DispatchAsync<UpdateShipmentChemicalCommand, Result<bool>>(command, HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
