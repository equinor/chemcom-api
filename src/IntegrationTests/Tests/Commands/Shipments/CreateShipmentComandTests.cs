using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using IntegrationTests.Common;
using IntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Tests.Commands.Shipments;

[Collection("TestSetupCollection")]
public sealed class CreateShipmentComandTests
{
    //TODO: create tests for onshore and offshore shipments
    private readonly TestSetupFixture _testSetupFixture;
    public CreateShipmentComandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchCreateShipmentShouldBeSuccess()
    {
        CreateShipmentCommand command = new CreateShipmentCommand()
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
            ShipmentParts = new List<int> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> result = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(command);

        Assert.True(result.Status == ResultStatusConstants.Success);
        Assert.True(result.Data is not null);
        Assert.True(result.Errors is null);
    }


    [Fact]
    public async Task DispatchShouldNotCreateShipment_WithValidationFailing()
    {
        CreateShipmentCommand command = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Guid.Empty,
            ReceiverId = new Guid("c4d2d827-48e6-45a8-9fb4-dbd8e7a54a67"),
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = false,
            PlannedExecutionFrom = null,
            PlannedExecutionTo = null,
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<int> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> result = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(command);

        Assert.True(result.Status == ResultStatusConstants.Failed);
        Assert.True(result.Data is null);
        Assert.True(result.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.SenderRequiredText, result.Errors);
        Assert.Contains(ShipmentValidationErrors.PlannedExecutionFromDateRequiredText, result.Errors);
        Assert.Contains(ShipmentValidationErrors.PlannedExecutionToDateRequiredText, result.Errors);
        Assert.Contains(ShipmentValidationErrors.UserAccessForInstallationText, result.Errors);
    }

    [Fact]
    public async Task DispatchShouldCreateShipment_ShipmentPartsValidationFailing()
    {
        CreateShipmentCommand command = new CreateShipmentCommand()
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
            ShipmentParts = new List<int> { 1, 1, 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> result = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(command);

        Assert.True(result.Status == ResultStatusConstants.Failed);
        Assert.True(result.Data is null);
        Assert.Contains(ShipmentValidationErrors.ShipmentPartsDaysDoesNotMatchText, result.Errors);

    }
}
