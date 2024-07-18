using Application.Comments.Commands.Create;
using Application.Comments.Queries.GetCommentsByShipmentId;
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

namespace IntegrationTests.Tests.Queries.Comments;

[Collection("TestSetupCollection")]
public sealed class GetCommentsByShipmentIdTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public GetCommentsByShipmentIdTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldReturnCommentsByShipmentId()
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
            User = user,
        };

        Result<Guid> createShipmentResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(command);


        await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateCommentCommand, Result<CreateCommentResult>>(
            new CreateCommentCommand("Test comment A", createShipmentResult.Data, user));
        await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateCommentCommand, Result<CreateCommentResult>>(
          new CreateCommentCommand("Test comment B", createShipmentResult.Data, user));

        GetCommentsByShipmentIdQuery query = new GetCommentsByShipmentIdQuery(createShipmentResult.Data);
        Result<GetCommentsByShipmentIdResult> getCommentsResult = await _testSetupFixture.QueryDispatcher.DispatchAsync<GetCommentsByShipmentIdQuery, Result<GetCommentsByShipmentIdResult>>(query);

        Assert.True(getCommentsResult.Status == ResultStatusConstants.Success);
        Assert.True(getCommentsResult.Data is not null);
        Assert.True(getCommentsResult.Errors is null);
    }
}
