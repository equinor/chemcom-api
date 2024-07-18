using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Application.Shipments.Queries.GeyShipmentById;
using Domain.Users;
using IntegrationTests.Common;
using IntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Tests.Queries.Shipments;

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

        Result<Guid> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(command);

        Result<GetShipmentByIdQueryResult> getResult = await _testSetupFixture.QueryDispatcher.DispatchAsync<GetShipmentByIdQuery, Result<GetShipmentByIdQueryResult>>(new GetShipmentByIdQuery(createResult.Data));

        Assert.True(getResult.Status == ResultStatusConstants.Success);
        Assert.True(getResult.Data is not null);
        Assert.True(getResult.Errors is null);
    }
}
