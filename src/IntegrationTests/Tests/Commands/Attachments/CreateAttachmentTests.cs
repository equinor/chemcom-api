using Application.Attachments.Commands.Create;
using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
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
        CreateShipmentCommand command = new CreateShipmentCommand()
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
            ShipmentParts = new List<double> { 1 },
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
        };
        Result<CreateShipmentResult> createResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(command);

        CreateAttachmentCommand createAttachmentCommand = new(createResult.Data.Id, "C:/", "jpg", "image/jpeg", new byte['f'], "ABCD@equinor.com", "ABCD");
        Result<CreateAttachmentResult> createAttachmentResult = await _testSetupFixture
            .CommandDispatcher
            .DispatchAsync<CreateAttachmentCommand, Result<CreateAttachmentResult>>(createAttachmentCommand);

        Assert.True(createAttachmentResult.Status == ResultStatusConstants.Success);
        Assert.True(createAttachmentResult.Data is not null);
        Assert.True(createAttachmentResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldNotCreateAttachmentReturnsShipmentNotFound()
    {
        CreateAttachmentCommand createAttachmentCommand = new(Guid.NewGuid(), "C:/", "jpg", "image/jpeg", new byte['f'], "ABCD@equinor.com", "ABCD");
        Result<CreateAttachmentResult> createAttachmentResult = await _testSetupFixture
                    .CommandDispatcher
                    .DispatchAsync<CreateAttachmentCommand, Result<CreateAttachmentResult>>(createAttachmentCommand);

        Assert.True(createAttachmentResult.Status == ResultStatusConstants.NotFound);
        Assert.True(createAttachmentResult.Data is null);
        Assert.True(createAttachmentResult.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.ShipmentNotFoundText, createAttachmentResult.Errors);
    }

    //TODO: File upload failed Test. Might have to use mock for this
}
