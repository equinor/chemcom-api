using Application.Chemicals.Commands.AddShipmentChemical;
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

namespace ChemDec.Api.Endpoints.Shipments.Chemicals.Add;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Add : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;
    public Add(ICommandDispatcher commandDispatcher, UserService userService)
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
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromBody] AddShipmentChemicalRequest request)
    {
        User user = await _userService.GetUser(User);
        AddShipmentChemicalCommand command = new AddShipmentChemicalCommand()
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

        Result<Guid> result = await _commandDispatcher.DispatchAsync<AddShipmentChemicalCommand, Result<Guid>>(command, HttpContext.RequestAborted);
        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        string createdAt = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host.ToUriComponent()}{HttpContext.Request.Path.ToUriComponent()}/{result.Data}";
        return Created(createdAt, result);
    }
}
