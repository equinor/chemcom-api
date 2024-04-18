using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Queries;
using IntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Tests.Commands.Shipments;

[Collection("TestSetupCollection")]
public sealed class GetShipmentByIdQueryHandlerTests
{
    private readonly TestSetupFixture _testSetupFixture;
    public GetShipmentByIdQueryHandlerTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldGetShipmentById()
    {
        bool isInstallationPartOfUserRoles = true;

        CreateShipmentCommand command = new CreateShipmentCommand()
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
            WaterAmount = 3,
            WaterAmountPerHour = 0,
            Well = "test",
            ShipmentParts = new List<int> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(command);

        Result<GetShipmentByIdQueryResult> getResult = await _testSetupFixture.QueryDispatcher.DispatchAsync<GetShipmentByIdQuery, Result<GetShipmentByIdQueryResult>>(new GetShipmentByIdQuery(createResult.Data.Id));

        Assert.True(getResult.Status == ResultStatusConstants.Success);
    }
}
