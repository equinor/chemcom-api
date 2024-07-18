using Application.Comments.Commands.Create;
using Application.Comments.Commands.Delete;
using Application.Common;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using Domain.Users;
using IntegrationTests.Common;
using IntegrationTests.Fixtures;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Tests.Commands.Comments;

[Collection("TestSetupCollection")]
public sealed class DeleteCommentCommandTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public DeleteCommentCommandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldDeleteCommant()
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

        Result<Guid> createCommentResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateCommentCommand, Result<Guid>>(
            new CreateCommentCommand("Test comment A", createShipmentResult.Data, user));

        DeleteCommentCommand deleteCommentCommand = new DeleteCommentCommand(createCommentResult.Data, createShipmentResult.Data, user);
        Result<bool> deleteCommentResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<DeleteCommentCommand, Result<bool>>(deleteCommentCommand);

        Assert.True(deleteCommentResult.Status == ResultStatusConstants.Success);
        Assert.True(deleteCommentResult.Data);
        Assert.True(deleteCommentResult.Errors is null);
    }
}
