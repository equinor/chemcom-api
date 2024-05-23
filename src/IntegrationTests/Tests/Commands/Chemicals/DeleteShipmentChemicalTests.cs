using Application.Chemicals.Commands.AddShipmentChemical;
using Application.Chemicals.Commands.Create;
using Application.Chemicals.Commands.DeleteShipmentChemical;
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
        Result<CreateShipmentResult> createShipmentResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);

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
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD"
        };

        Result<List<Guid>> addShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<AddShipmentChemicalsCommand, Result<List<Guid>>>(addShipmentChemicalCommand);

        DeleteShipmentChemicalCommand deleteShipmentChemicalCommand = new(addShipmentChemicalResult.Data.First(), createShipmentResult.Data.Id, "abcd@equinor.com", "ABCD");
        Result<bool> deleteShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<DeleteShipmentChemicalCommand, Result<bool>>(deleteShipmentChemicalCommand);

        Assert.True(deleteShipmentChemicalResult.Status == ResultStatusConstants.Success);
        Assert.True(deleteShipmentChemicalResult.Data);
        Assert.True(deleteShipmentChemicalResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldNotDeleteShipmentChemicalReturnShipmentNotFound()
    {

        Guid shipmentId = Guid.NewGuid();
        Guid shipmenChemicalId = Guid.NewGuid();

        DeleteShipmentChemicalCommand deleteShipmentChemicalCommand = new(shipmenChemicalId, shipmentId, "abcd@equinor.com", "ABCD");
        Result<bool> deleteShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<DeleteShipmentChemicalCommand, Result<bool>>(deleteShipmentChemicalCommand);

        Assert.True(deleteShipmentChemicalResult.Status == ResultStatusConstants.NotFound);
        Assert.True(deleteShipmentChemicalResult.Data == false);
        Assert.True(deleteShipmentChemicalResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.ShipmentNotFoundText, deleteShipmentChemicalResult.Errors);
    }
}
