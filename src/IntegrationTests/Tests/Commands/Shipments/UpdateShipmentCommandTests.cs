﻿using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Update;
using Domain.Users;
using IntegrationTests.Common;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Tests.Commands.Shipments;

[Collection("TestSetupCollection")]
public sealed class UpdateShipmentCommandTests
{
    private readonly TestSetupFixture _testSetupFixture;
    public UpdateShipmentCommandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldUpdateShipment()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateShipmentCommand createCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            Type = "wellintervention",
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 1,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user
        };

        Result<Guid> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(createCommand);

        UpdateShipmentCommand updateCommand = new()
        {
            Id = createResult.Data,
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            Type = "wellintervention",
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 2,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user
        };

        Result<Guid> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateShipmentCommand, Result<Guid>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.Success);
        Assert.True(updateResult.Data != Guid.Empty);
        Assert.True(updateResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldNotUpdateShipmentWithNotFound()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        UpdateShipmentCommand updateCommand = new()
        {
            Id = Guid.NewGuid(),
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            Type = "wellintervention",
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 2,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user
        };

        Result<Guid> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateShipmentCommand, Result<Guid>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.NotFound);
        Assert.True(updateResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.ShipmentNotFoundText, updateResult.Errors);
    }

    [Fact]
    public async Task DispatchShouldNotUpdateWithValidationFailing()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateShipmentCommand createCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            Type = "wellintervention",
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 1,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user
        };

        Result<Guid> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(createCommand);

        UpdateShipmentCommand updateCommand = new()
        {
            Id = createResult.Data,
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Guid.Empty,
            Type = "wellintervention",
            PlannedExecutionFrom = null,
            PlannedExecutionTo = null,
            WaterAmount = 2,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user
        };

        Result<Guid> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateShipmentCommand, Result<Guid>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.Failed);
        Assert.True(updateResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.SenderRequiredText, updateResult.Errors);
    }
}
