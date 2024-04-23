using Application.Chemicals.Commands.AddChemicalToShipment;
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

namespace ChemDec.Api.Endpoints.Chemicals.AddChemicalToShipment;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class AddChemicalToShipment : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;
    public AddChemicalToShipment(ICommandDispatcher commandDispatcher, UserService userService)
    {
        _commandDispatcher = commandDispatcher;
        _userService = userService;

    }

    [HttpPost("{shipmentId}/chemicals")]
    [SwaggerOperation(Description = "Add chemical to shipment",
                               Summary = "Add chemical to shipment",
                               Tags = new[] { "Shipments - new" })]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromBody] AddChemicalToShipmentRequest request)
    {
        User user = await _userService.GetUser(User);
        AddChemicalToShipmentCommand command = new AddChemicalToShipmentCommand()
        {
            ShipmentId = shipmentId,
            ChemicalId = request.ChemicalId,
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

        Result<Guid> result = await _commandDispatcher.DispatchAsync<AddChemicalToShipmentCommand, Result<Guid>>(command, HttpContext.RequestAborted);
        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        Uri createdAt = new Uri($"{HttpContext.Request.Host}/api/shipments/{shipmentId}/chemicals/{result.Data}");
        return Created(createdAt, result);
    }
}
