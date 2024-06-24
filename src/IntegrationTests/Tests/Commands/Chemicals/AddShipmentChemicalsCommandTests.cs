using Application.Chemicals.Commands.AddShipmentChemical;
using Application.Chemicals.Commands.Create;
using Application.Chemicals.Commands.DeleteShipmentChemical;
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

namespace IntegrationTests.Tests.Commands.Chemicals;

[Collection("TestSetupCollection")]
public sealed class AddShipmentChemicalsCommandTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public AddShipmentChemicalsCommandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldAddShipmentChemicals()
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
        Result<CreateShipmentResult> createShipmentResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);

        CreateChemicalCommand createChemicalCommand = new CreateChemicalCommand
        {
            Name = "Testing adding chemical",
            Description = "Testing adding chemical Description",
            Tentative = false,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<CreateChemicalResult> createChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(createChemicalCommand);

        List<ShipmentChemicalItem> shipmentChemicalItems =
        [
            new ShipmentChemicalItem
            {
                ChemicalId = createChemicalResult.Data.Id,
                Amount = 10,
                MeasureUnit = "kg",
            },
        ];

        AddShipmentChemicalsCommand addShipmentChemicalCommand = new()
        {
            ShipmentId = createShipmentResult.Data.Id,
            ShipmentChemicalItems = shipmentChemicalItems,
            User = user
        };

        Result<List<Guid>> addShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<AddShipmentChemicalsCommand, Result<List<Guid>>>(addShipmentChemicalCommand);

        Assert.True(addShipmentChemicalResult.Status == ResultStatusConstants.Success);
        Assert.True(addShipmentChemicalResult.Data.Any());
        Assert.True(addShipmentChemicalResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldNotAddShipmentChemicalReturnShipmentNotFound()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);

        CreateChemicalCommand createChemicalCommand = new CreateChemicalCommand
        {
            Name = "adding chemical 200",
            Description = "Testing deleting chemical Description",
            Tentative = false,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<CreateChemicalResult> createChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(createChemicalCommand);

        List<ShipmentChemicalItem> shipmentChemicalItems =
         [
             new ShipmentChemicalItem
            {
                ChemicalId = createChemicalResult.Data.Id,
                Amount = 10,
                MeasureUnit = "kg",
            },
        ];

        AddShipmentChemicalsCommand addShipmentChemicalCommand = new()
        {
            ShipmentId = Guid.NewGuid(),
            ShipmentChemicalItems = shipmentChemicalItems,
            User = user
        };

        Result<List<Guid>> addShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<AddShipmentChemicalsCommand, Result<List<Guid>>>(addShipmentChemicalCommand);


        Assert.True(addShipmentChemicalResult.Status == ResultStatusConstants.NotFound);
        Assert.True(addShipmentChemicalResult.Data is null);
        Assert.True(addShipmentChemicalResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.ShipmentNotFoundText, addShipmentChemicalResult.Errors);
    }

    [Fact]
    public async Task DispatchShouldNotAddShipmentChemicalReturnInvalidMeasureUnit()
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
        Result<CreateShipmentResult> createShipmentResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);

        CreateChemicalCommand createChemicalCommand = new CreateChemicalCommand
        {
            Name = "adding chemical 100",
            Description = "Testing deleting chemical Description",
            Tentative = false,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<CreateChemicalResult> createChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(createChemicalCommand);

        List<ShipmentChemicalItem> shipmentChemicalItems =
         [
             new ShipmentChemicalItem
            {
                ChemicalId = createChemicalResult.Data.Id,
                Amount = 10,
                MeasureUnit = "kgc",
            },
        ];

        AddShipmentChemicalsCommand addShipmentChemicalCommand = new()
        {
            ShipmentId = createShipmentResult.Data.Id,
            ShipmentChemicalItems = shipmentChemicalItems,
            User = user
        };

        Result<List<Guid>> addShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<AddShipmentChemicalsCommand, Result<List<Guid>>>(addShipmentChemicalCommand);


        Assert.True(addShipmentChemicalResult.Status == ResultStatusConstants.Failed);
        Assert.True(addShipmentChemicalResult.Data is null);
        Assert.True(addShipmentChemicalResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.InvalidMeasureUnitText, addShipmentChemicalResult.Errors);
    }
}
