using Application.Chemicals.Commands.AddShipmentChemical;
using Application.Chemicals.Commands.Create;
using Application.Chemicals.Commands.Update;
using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
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

        Result<CreateChemicalResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(createCommand);

        UpdateChemicalCommand updateCommand = new()
        {
            Id = createResult.Data.Id,
            Name = "Test Chemical",
            Description = "Test Description",
            Tentative = true,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<UpdateChemicalResult> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateChemicalCommand, Result<UpdateChemicalResult>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.Success);
        Assert.True(updateResult.Data is not null);
        Assert.True(updateResult.Errors is null);
    }

    [Fact(Skip = "This is not related to new shipment creation. Will comeback to this when working on chemical creation page")]    
    public async Task DispatchShouldUpdateChemicalWithShipmentChemicals()
    {
        CreateShipmentCommand command = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = new Guid("b10fc741-ebe3-45c1-bc70-8eca5ba5ced6"),
            ReceiverId = new Guid("c4d2d827-48e6-45a8-9fb4-dbd8e7a54a67"),
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

        Result<CreateShipmentResult> createShipment = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(command);

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

        Result<CreateChemicalResult> createChemicalResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(createCommand);

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
            ShipmentId = createShipment.Data.Id,
            ShipmentChemicalItems = shipmentChemicalItems,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD"
        };      

        Result<Guid> addShipmentChemicalResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<AddShipmentChemicalsCommand, Result<Guid>>(addShipmentChemicalCommand);


        UpdateChemicalCommand updateCommand = new()
        {
            Id = createChemicalResult.Data.Id,
            Name = "Test Chemical",
            Description = "Test Description",
            Tentative = true,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<UpdateChemicalResult> updateResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<UpdateChemicalCommand, Result<UpdateChemicalResult>>(updateCommand);

        Assert.True(updateResult.Status == ResultStatusConstants.Success);
        Assert.True(updateResult.Data is not null);
        Assert.True(updateResult.Errors is null);
    }
}
