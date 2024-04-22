using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Update;
using IntegrationTests.Fixtures;
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
        bool isInstallationPartOfUserRoles = true;

        CreateShipmentCommand createCommand = new CreateShipmentCommand()
        {
            Code = "pov",
            Title = "Test bon integration test",
            SenderId = new Guid("b10fc741-ebe3-45c1-bc70-8eca5ba5ced6"),
            ReceiverId = new Guid("c4d2d827-48e6-45a8-9fb4-dbd8e7a54a67"),
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = isInstallationPartOfUserRoles,
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
            SenderId = new Guid("b10fc741-ebe3-45c1-bc70-8eca5ba5ced6"),
            ReceiverId = new Guid("c4d2d827-48e6-45a8-9fb4-dbd8e7a54a67"),
            Type = "wellintervention",
            Initiator = Initiator.Offshore,
            IsInstallationPartOfUserRoles = isInstallationPartOfUserRoles,
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
}
