using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Submit;
using Domain.Users;
using IntegrationTests.Common;
using IntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Tests.Commands.Shipments;

[Collection("TestSetupCollection")]
public sealed class SubmitShipmentCommandTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public SubmitShipmentCommandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldSubmitShipment()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateShipmentCommand createShipmentCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            Type = "wellintervention",
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);


        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = createResult.Data.Id,
            User = user,
            TakePrecaution = false,
            HeightenedLra = false,
            AvailableForDailyContact = false
        };

        Result<bool> submitResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<SubmitShipmentCommand, Result<bool>>(submitShipmentCommand);

        Assert.True(submitResult.Status == ResultStatusConstants.Success);
        Assert.True(submitResult.Data);
        Assert.True(submitResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldSubmitShipmentWithHeightenedLra()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateShipmentCommand createShipmentCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            Type = "wellintervention",
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);


        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = createResult.Data.Id,
            User = user,
            TakePrecaution = false,
            HeightenedLra = true,
            Pb210 = 1,
            Ra226 = 1,
            Ra228 = 1,
            AvailableForDailyContact = false
        };

        Result<bool> submitResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<SubmitShipmentCommand, Result<bool>>(submitShipmentCommand);

        Assert.True(submitResult.Status == ResultStatusConstants.Success);
        Assert.True(submitResult.Data);
        Assert.True(submitResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldReturnNotFound()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = Guid.NewGuid(),
            User = user,
            TakePrecaution = false,
            HeightenedLra = false,
            AvailableForDailyContact = false
        };

        Result<bool> submitResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<SubmitShipmentCommand, Result<bool>>(submitShipmentCommand);

        Assert.True(submitResult.Status == ResultStatusConstants.NotFound);
        Assert.True(!submitResult.Data);
        Assert.True(submitResult.Errors is not null);
    }

    [Fact]
    public async Task DispatchShouldNotAllowReSubmit()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateShipmentCommand createShipmentCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            Type = "wellintervention",
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);


        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = createResult.Data.Id,
            User = user,
            TakePrecaution = false,
            HeightenedLra = false,
            AvailableForDailyContact = false
        };

        Result<bool> submitResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<SubmitShipmentCommand, Result<bool>>(submitShipmentCommand);

        Result<bool> reSubmitResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<SubmitShipmentCommand, Result<bool>>(submitShipmentCommand);

        Assert.True(reSubmitResult.Status == ResultStatusConstants.Failed);
        Assert.True(!reSubmitResult.Data);
        Assert.True(reSubmitResult.Errors is not null);
    }

    [Fact]
    public async Task DispatchShouldSubmitShipmentWithTakePrecautions()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);

        CreateShipmentCommand createShipmentCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            Type = "wellintervention",
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user,
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);


        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = createResult.Data.Id,
            User = user,
            TakePrecaution = true,
            Precautions = "Test precautions",
            HeightenedLra = true,
            Pb210 = 1,
            Ra226 = 1,
            Ra228 = 1,
            AvailableForDailyContact = false
        };

        Result<bool> submitResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<SubmitShipmentCommand, Result<bool>>(submitShipmentCommand);

        Assert.True(submitResult.Status == ResultStatusConstants.Success);
        Assert.True(submitResult.Data);
        Assert.True(submitResult.Errors is null);
    }
}
