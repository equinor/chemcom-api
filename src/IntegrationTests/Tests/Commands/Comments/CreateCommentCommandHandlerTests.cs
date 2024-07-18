using Application.Comments.Commands.Create;
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

namespace IntegrationTests.Tests.Commands.Comments;

[Collection("TestSetupCollection")]
public sealed class CreateCommentCommandHandlerTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public CreateCommentCommandHandlerTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldCreateComment()
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
        Result<Guid> createCommentResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateCommentCommand, Result<Guid>>(
            new CreateCommentCommand("Test comment", createResult.Data, user));

        Assert.True(createCommentResult.Status == ResultStatusConstants.Success);
        Assert.True(createCommentResult.Data != Guid.Empty);
        Assert.True(createCommentResult.Errors is null);
    }
}
