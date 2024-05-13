using Application.Attachments.Commands.Create;
using Application.Chemicals.Commands.AddChemicalToShipment;
using Application.Chemicals.Commands.Create;
using Application.Common;
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
public sealed class AddShipmentChemicalCommandTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public AddShipmentChemicalCommandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldAddShipmentChemical()
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

        AddChemicalToShipmentCommand addShipmentChemicalCommand = new()
        {
            ShipmentId = createShipmentResult.Data.Id,
            ChemicalId = createChemicalResult.Data.Id,
            Amount = 10,
            MeasureUnit = "kg"
        };

        Result<Guid> addShipmentChemicalResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<AddChemicalToShipmentCommand, Result<Guid>>(addShipmentChemicalCommand);

        Assert.True(addShipmentChemicalResult.Status == ResultStatusConstants.Success);
        Assert.True(addShipmentChemicalResult.Data != Guid.Empty);
        Assert.True(addShipmentChemicalResult.Errors is null);
    }
}
