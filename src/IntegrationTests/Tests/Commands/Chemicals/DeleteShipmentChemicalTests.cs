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
public sealed class DeleteShipmentChemicalTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public DeleteShipmentChemicalTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldDeleteShipmentChemical()
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
        Result<Guid> createShipmentResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(createShipmentCommand);

        CreateChemicalCommand createChemicalCommand = new CreateChemicalCommand
        {
            Name = "Testing deleting chemical",
            Description = "Testing deleting chemical Description",
            Tentative = false,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<Guid> createChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<Guid>>(createChemicalCommand);

        List<ShipmentChemicalItem> shipmentChemicalItems =
        [
            new ShipmentChemicalItem
            {
                ChemicalId = createChemicalResult.Data,
                Amount = 10,
                MeasureUnit = "kg",
            },
        ];

        AddShipmentChemicalsCommand addShipmentChemicalCommand = new()
        {
            ShipmentId = createShipmentResult.Data,
            ShipmentChemicalItems = shipmentChemicalItems,
            User = user
        };

        Result<List<Guid>> addShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<AddShipmentChemicalsCommand, Result<List<Guid>>>(addShipmentChemicalCommand);

        DeleteShipmentChemicalCommand deleteShipmentChemicalCommand = new(addShipmentChemicalResult.Data.First(), createShipmentResult.Data, user);
        Result<bool> deleteShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<DeleteShipmentChemicalCommand, Result<bool>>(deleteShipmentChemicalCommand);

        Assert.True(deleteShipmentChemicalResult.Status == ResultStatusConstants.Success);
        Assert.True(deleteShipmentChemicalResult.Data);
        Assert.True(deleteShipmentChemicalResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldNotDeleteShipmentChemicalReturnShipmentNotFound()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        Guid shipmentId = Guid.NewGuid();
        Guid shipmenChemicalId = Guid.NewGuid();

        DeleteShipmentChemicalCommand deleteShipmentChemicalCommand = new(shipmenChemicalId, shipmentId, user);
        Result<bool> deleteShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<DeleteShipmentChemicalCommand, Result<bool>>(deleteShipmentChemicalCommand);

        Assert.True(deleteShipmentChemicalResult.Status == ResultStatusConstants.NotFound);
        Assert.True(deleteShipmentChemicalResult.Data == false);
        Assert.True(deleteShipmentChemicalResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.ShipmentNotFoundText, deleteShipmentChemicalResult.Errors);
    }
}
