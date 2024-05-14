using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Submit;
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
        CreateShipmentCommand createShipmentCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            ReceiverId = Constants.ReceiverId,
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = true,
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);


        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = createResult.Data.Id,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
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
        CreateShipmentCommand createShipmentCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            ReceiverId = Constants.ReceiverId,
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = true,
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);


        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = createResult.Data.Id,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
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
        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = Guid.NewGuid(),
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
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
        CreateShipmentCommand createShipmentCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            ReceiverId = Constants.ReceiverId,
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = true,
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);


        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = createResult.Data.Id,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
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
        CreateShipmentCommand createShipmentCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            ReceiverId = Constants.ReceiverId,
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = true,
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);


        SubmitShipmentCommand submitShipmentCommand = new SubmitShipmentCommand
        {
            ShipmentId = createResult.Data.Id,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
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
