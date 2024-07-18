using Application.Chemicals.Commands.Create;
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

namespace ChemDec.Api.Endpoints.Chemicals.Create;

[Route("api/chemicals")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class Create : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;

    public Create(ICommandDispatcher commandDispatcher, UserService userService)
    {
        _commandDispatcher = commandDispatcher;
        _userService = userService;
    }

    [HttpPost]
    [SwaggerOperation(Description = "Create new chemical",
                               Summary = "Create new chemical",
                               Tags = ["Chemicals - new"])]
    [ProducesResponseType(typeof(Result<Guid>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> HandleAsync([FromBody] CreateChemicalRequest request)
    {
        User user = await _userService.GetUser(User);

        CreateChemicalCommand command = new CreateChemicalCommand()
        {
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

        Result<Guid> result = await _commandDispatcher.DispatchAsync<CreateChemicalCommand, Result<Guid>>(command, HttpContext.RequestAborted);

        if (result.Status == ResultStatusConstants.Failed)
        {
            return BadRequest(result);
        }

        Uri createdAt = new Uri($"{HttpContext.Request.Host}/api/chemicals/{result.Data}");
        return Created(createdAt, result);
    }
}
