using Application.Chemicals.Commands.AddShipmentChemical;
using Application.Chemicals.Commands.Create;
using Application.Chemicals.Commands.Update;
using Application.Common;
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
public sealed class UpdateChemicalCommandTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public UpdateChemicalCommandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldUpdateChemical()
    {
        CreateChemicalCommand createCommand = new()
        {
            Name = "Test chemical update",
            Description = "Test description update",
            Tentative = true,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<Guid> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<Guid>>(createCommand);

        UpdateChemicalCommand updateCommand = new()
        {
            Id = createResult.Data,
            Name = "Test Chemical",
            Description = "Test Description",
            Tentative = true,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<Guid> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateChemicalCommand, Result<Guid>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.Success);
        Assert.True(updateResult.Data != Guid.Empty);
        Assert.True(updateResult.Errors is null);
    }

    [Fact(Skip = "This is not related to new shipment creation. Will comeback to this when working on chemical creation page")]
    public async Task DispatchShouldUpdateChemicalWithShipmentChemicals()
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
            ShipmentParts = new List<double> { 1 },
            User = user
        };

        Result<Guid> createShipment = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(command);

        CreateChemicalCommand createCommand = new()
        {
            Name = "Test Chemical",
            Description = "Test Description",
            Tentative = true,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<Guid> createChemicalResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<Guid>>(createCommand);

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
            ShipmentId = createShipment.Data,
            ShipmentChemicalItems = shipmentChemicalItems,
            User = user
        };

        Result<Guid> addShipmentChemicalResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<AddShipmentChemicalsCommand, Result<Guid>>(addShipmentChemicalCommand);

        UpdateChemicalCommand updateCommand = new()
        {
            Id = createChemicalResult.Data,
            Name = "Test Chemical",
            Description = "Test Description",
            Tentative = true,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<Guid> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateChemicalCommand, Result<Guid>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.Success);
        Assert.True(updateResult.Data != Guid.Empty);
        Assert.True(updateResult.Errors is null);
    }
}
