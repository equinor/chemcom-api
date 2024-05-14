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
            ShipmentParts = new List<int> { 1 },
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

        AddShipmentChemicalCommand addShipmentChemicalCommand = new()
        {
            ShipmentId = createShipmentResult.Data.Id,
            ChemicalId = createChemicalResult.Data.Id,
            Amount = 10,
            MeasureUnit = "kg"
        };

        Result<Guid> addShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<AddShipmentChemicalCommand, Result<Guid>>(addShipmentChemicalCommand);

        DeleteShipmentChemicalCommand deleteShipmentChemicalCommand = new(addShipmentChemicalResult.Data, createShipmentResult.Data.Id, "abcd@equinor.com", "ABCD");
        Result<bool> deleteShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<DeleteShipmentChemicalCommand, Result<bool>>(deleteShipmentChemicalCommand);

        Assert.True(deleteShipmentChemicalResult.Status == ResultStatusConstants.Success);
        Assert.True(deleteShipmentChemicalResult.Data);
        Assert.True(deleteShipmentChemicalResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldNotDeleteShipmentChemicalReturnShipmentNotFound()
    {
        CreateChemicalCommand createChemicalCommand = new CreateChemicalCommand
        {
            Name = "Deleting chemical not foud",
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

        Guid shipmentId = Guid.NewGuid();

        AddShipmentChemicalCommand addShipmentChemicalCommand = new()
        {
            ShipmentId = shipmentId,
            ChemicalId = createChemicalResult.Data.Id,
            Amount = 10,
            MeasureUnit = "kg"
        };

        Result<Guid> addShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<AddShipmentChemicalCommand, Result<Guid>>(addShipmentChemicalCommand);

        DeleteShipmentChemicalCommand deleteShipmentChemicalCommand = new(addShipmentChemicalResult.Data, shipmentId, "abcd@equinor.com", "ABCD");
        Result<bool> deleteShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<DeleteShipmentChemicalCommand, Result<bool>>(deleteShipmentChemicalCommand);

        Assert.True(deleteShipmentChemicalResult.Status == ResultStatusConstants.NotFound);
        Assert.True(deleteShipmentChemicalResult.Data == false);
        Assert.True(deleteShipmentChemicalResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.ShipmentNotFoundText, deleteShipmentChemicalResult.Errors);
    }
}
