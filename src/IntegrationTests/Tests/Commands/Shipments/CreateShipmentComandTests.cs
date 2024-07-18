using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
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
public sealed class CreateShipmentComandTests
{
    private readonly TestSetupFixture _testSetupFixture;
    public CreateShipmentComandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchCreateShipmentShouldBeSuccess()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateShipmentCommand command = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Constants.SenderId,
            Type = "wellintervention",
            PlannedExecutionFrom = new DateTime(2024, 05, 15, 13, 14, 25),
            PlannedExecutionTo = new DateTime(2024, 05, 16, 02, 0, 0),
            WaterAmount = 2,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1, 1 },
            User = user
        };

        Result<Guid> result = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(command);

        Assert.True(result.Status == ResultStatusConstants.Success);
        Assert.True(result.Data != Guid.Empty);
        Assert.True(result.Errors is null);
    }


    [Fact]
    public async Task DispatchShouldNotCreateShipment_WithValidationFailing()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateShipmentCommand command = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = Guid.Empty,
            Type = "wellintervention",
            PlannedExecutionFrom = null,
            PlannedExecutionTo = null,
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<double> { 1 },
            User = user,
        };

        Result<Guid> result = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(command);

        Assert.True(result.Status == ResultStatusConstants.Failed);
        Assert.True(result.Data == Guid.Empty);
        Assert.True(result.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.SenderRequiredText, result.Errors);
        //Assert.Contains(ShipmentValidationErrors.PlannedExecutionFromDateRequiredText, result.Errors);
        //Assert.Contains(ShipmentValidationErrors.PlannedExecutionToDateRequiredText, result.Errors);
        Assert.Contains(ShipmentValidationErrors.UserAccessForInstallationText, result.Errors);
    }

    [Fact]
    public async Task DispatchShouldCreateShipment_ShipmentPartsValidationFailing()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateShipmentCommand command = new CreateShipmentCommand()
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
            ShipmentParts = new List<double> { 1, 1, 1 },
            User = user
        };

        Result<Guid> result = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(command);

        Assert.True(result.Status == ResultStatusConstants.Failed);
        Assert.True(result.Data == Guid.Empty);
        Assert.Contains(ShipmentValidationErrors.ShipmentPartsDaysDoesNotMatchText, result.Errors);
    }
}
