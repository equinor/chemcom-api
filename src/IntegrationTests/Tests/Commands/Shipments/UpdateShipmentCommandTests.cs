using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Update;
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
        CreateShipmentCommand createCommand = new CreateShipmentCommand()
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
            WaterAmount = 1,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<int> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createCommand);

        Dictionary<Guid, double> shipmentParts = new()
        {
            { Guid.NewGuid(), 2.0 }
        };

        UpdateShipmentCommand updateCommand = new()
        {
            Id = createResult.Data.Id,
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            ReceiverId = Constants.ReceiverId,
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = true,
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 2,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = shipmentParts,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<UpdateShipmentResult> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateShipmentCommand, Result<UpdateShipmentResult>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.Success);
        Assert.True(updateResult.Data is not null);
        Assert.True(updateResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldNotUpdateShipmentWithNotFound()
    {
        Dictionary<Guid, double> shipmentParts = new()
        {
            { Guid.NewGuid(), 2.0 }
        };

        UpdateShipmentCommand updateCommand = new()
        {
            Id = Guid.NewGuid(),
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            ReceiverId = Constants.ReceiverId,
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = true,
            PlannedExecutionFrom = new DateTime(2024, 3, 15),
            PlannedExecutionTo = new DateTime(2024, 3, 15),
            WaterAmount = 2,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = shipmentParts,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<UpdateShipmentResult> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateShipmentCommand, Result<UpdateShipmentResult>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.NotFound);
        Assert.True(updateResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.ShipmentNotFoundText, updateResult.Errors);
    }

    [Fact]
    public async Task DispatchShouldNotUpdateWithValidationFailing()
    {
        CreateShipmentCommand createCommand = new CreateShipmentCommand()
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
            WaterAmount = 1,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<int> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createCommand);

        Dictionary<Guid, double> shipmentParts = new()
        {
            { Guid.NewGuid(), 2.0 }
        };

        UpdateShipmentCommand updateCommand = new()
        {
            Id = createResult.Data.Id,
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Guid.Empty,
            ReceiverId = Constants.ReceiverId,
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = false,
            PlannedExecutionFrom = null,
            PlannedExecutionTo = null,
            WaterAmount = 2,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = shipmentParts,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<UpdateShipmentResult> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateShipmentCommand, Result<UpdateShipmentResult>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.Failed);
        Assert.True(updateResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.SenderRequiredText, updateResult.Errors);
        Assert.Contains(ShipmentValidationErrors.PlannedExecutionFromDateRequiredText, updateResult.Errors);
        Assert.Contains(ShipmentValidationErrors.PlannedExecutionToDateRequiredText, updateResult.Errors);
        Assert.Contains(ShipmentValidationErrors.UserAccessForInstallationText, updateResult.Errors);
    }
}
