﻿using Application.Chemicals.Commands.Approve;
using Application.Common;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System;
using System.Threading.Tasks;

namespace ChemDec.Api.Endpoints.Chemicals.Approve;

[Route("api/chemicals")]
[Authorize]
[ApiController]
[EnableCors("_myAllowSpecificOrigins")]
public class ApproveChemical : ControllerBase
{
    private readonly ICommandDispatcher _commandDispatcher;
    private readonly UserService _userService;

    public ApproveChemical(ICommandDispatcher commandDispatcher, UserService userService)
    {
        _commandDispatcher = commandDispatcher;
        _userService = userService;
    }

    [HttpPatch("{id}")]
    [SwaggerOperation(Description = "Approve chemical",
                        Summary = "Approve chemical",
                        Tags = new[] { "Chemicals - new" })]
    public async Task<IActionResult> HandleAsync([FromRoute] Guid id)
    {
        User user = await _userService.GetUser(User);

        ApproveChemicalCommand command = new ApproveChemicalCommand(id, user.Email, user.Name);
        Result<bool> result = await _commandDispatcher.DispatchAsync<ApproveChemicalCommand, Result<bool>>(command);

        if (result.Status == ResultStatusConstants.NotFound)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}
