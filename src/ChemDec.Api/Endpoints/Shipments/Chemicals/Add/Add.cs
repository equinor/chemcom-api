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
using System.Collections.Generic;
using System.Linq;
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
    [SwaggerOperation(Description = "Add/update chemicals to a shipment",
                               Summary = "Add/update chemicals to a shipment",
                               Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromBody] List<AddShipmentChemicalRequest> request)
    {
        User user = await _userService.GetUser(User);
        AddShipmentChemicalsCommand command = new AddShipmentChemicalsCommand()
        {
            ShipmentId = shipmentId,
            UpdatedByName = user.Name,
            UpdatedBy = user.Email
        };

        foreach (AddShipmentChemicalRequest item in request)
        {
            command.ShipmentChemicalItems.Add(
                new ShipmentChemicalItem
                {
                    ChemicalId = item.ChemicalId,
                    MeasureUnit = item.MeasureUnit,
                    Amount = item.Amount,
                    CalculatedWeightUnrinsed = item.CalculatedWeightUnrinsed,
                    CalculatedTocUnrinsed = item.CalculatedTocUnrinsed,
                    CalculatedNitrogenUnrinsed = item.CalculatedNitrogenUnrinsed,
                    CalculatedBiocidesUnrinsed = item.CalculatedBiocidesUnrinsed,
                    CalculatedWeight = item.CalculatedWeight,
                    CalculatedToc = item.CalculatedToc,
                    CalculatedNitrogen = item.CalculatedNitrogen,
                    CalculatedBiocides = item.CalculatedBiocides
                });
        }

        Result<List<Guid>> result = await _commandDispatcher.DispatchAsync<AddShipmentChemicalsCommand, Result<List<Guid>>>(command, HttpContext.RequestAborted);
        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        return Ok(result);
    }
}
