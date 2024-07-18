using Application.Attachments.Commands.Create;
using Application.Common;
using Application.Common.Constants;
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

namespace IntegrationTests.Tests.Commands.Attachments;

[Collection("TestSetupCollection")]
public class CreateAttachmentTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public CreateAttachmentTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldCreateAttachment()
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
        Result<Guid> createResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<Guid>>(command);

        CreateAttachmentCommand createAttachmentCommand = new(createResult.Data, "C:/", "jpg", "image/jpeg", new byte['f'], user);
        Result<Guid> createAttachmentResult = await _testSetupFixture
            .CommandDispatcher
            .DispatchAsync<CreateAttachmentCommand, Result<Guid>>(createAttachmentCommand);

        Assert.True(createAttachmentResult.Status == ResultStatusConstants.Success);
        Assert.True(createAttachmentResult.Data != Guid.Empty);
        Assert.True(createAttachmentResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldNotCreateAttachmentReturnsShipmentNotFound()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateAttachmentCommand createAttachmentCommand = new(Guid.NewGuid(), "C:/", "jpg", "image/jpeg", new byte['f'], user);
        Result<Guid> createAttachmentResult = await _testSetupFixture
                    .CommandDispatcher
                    .DispatchAsync<CreateAttachmentCommand, Result<Guid>>(createAttachmentCommand);

        Assert.True(createAttachmentResult.Status == ResultStatusConstants.NotFound);
        Assert.True(createAttachmentResult.Data == Guid.Empty);
        Assert.True(createAttachmentResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.ShipmentNotFoundText, createAttachmentResult.Errors);
    }

    //TODO: File upload failed Test. Might have to use mock for this
}
