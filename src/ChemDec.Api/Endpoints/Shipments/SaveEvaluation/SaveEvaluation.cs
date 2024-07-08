using Application.Common;
using Application.Shipments.Commands.Evaluate;
using Domain.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Shipments.SaveEvaluation;

[Route("api/shipments")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public sealed class SaveEvaluation : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly IUserProvider _userProvider;

    public SaveEvaluation(ICommandDispatcher commandDispatcher, IUserProvider userProvider)
    {
        _commandDispatcher = commandDispatcher;
        _userProvider = userProvider;
    }

    [HttpPatch("{shipmentId}/evaluation")]
    [SwaggerOperation(Description = "Update shipment evaluation",
                        Summary = "Update shipment evaluation",
                        Tags = ["Shipments - new"])]
    [ProducesResponseType(typeof(Result<bool>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ResultBase), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid shipmentId, [FromBody] SaveEvaluationRequest request)
    {
        User user = await _userProvider.GetUserAsync(User);

        SaveEvaluationCommand command = new()
        {
            ShipmentId = shipmentId,
            EvalAmountOk = request.EvalAmountOk,
            EvalBiocidesOk = request.EvalBiocidesOk,
            EvalCapacityOk = request.EvalCapacityOk,
            EvalContaminationRisk = request.EvalContaminationRisk,
            EvalEnvImpact = request.EvalEnvImpact,
            EvalComments = request.EvalComments,
            User = user
        };

        Result<bool> result = await _commandDispatcher.DispatchAsync<SaveEvaluationCommand, Result<bool>>(command, HttpContext.RequestAborted);

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
