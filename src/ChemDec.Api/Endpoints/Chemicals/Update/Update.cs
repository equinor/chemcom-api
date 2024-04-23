using Application.Chemicals.Commands.Update;
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

namespace ChemDec.Api.Endpoints.Chemicals.Update;

[Route("api/chemicals")]
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

    [HttpPut]
    [Route("{id}")]
    [SwaggerOperation(Description = "Update chemical",
                       Summary = "Update chemical",
                       Tags = new[] { "Chemicals - new" })]
    [ProducesResponseType(typeof(Result<UpdateChemicalResult>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid id, [FromBody] UpdateChemicalRequest request)
    {
        User user = await _userService.GetUser(User);

        UpdateChemicalCommand command = new UpdateChemicalCommand()
        {
            Id = id,
            Name = request.Name,
            Description = request.Description,
            Tentative = request.Tentative,
            Disabled = request.Disabled,
            ProposedByInstallationId = request.ProposedByInstallationId,
            ProposedBy = user.Email,
            ProposedByEmail = user.Email,
            ProposedByName = user.Name,
            TocWeight = request.TocWeight,
            NitrogenWeight = request.NitrogenWeight,
            Density = request.Density,
            UpdatedBy = user.Email,
            UpdatedByName = user.Name,
            HazardClass = request.HazardClass,
            FollowOilPhaseDefault = request.FollowOilPhaseDefault,
            FollowWaterPhaseDefault = request.FollowWaterPhaseDefault,
            BiocideWeight = request.BiocideWeight,
            MeasureUnitDefault = request.MeasureUnitDefault
        };

        Result<UpdateChemicalResult> result = await _commandDispatcher.DispatchAsync<UpdateChemicalCommand, Result<UpdateChemicalResult>>(command, HttpContext.RequestAborted);

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
