﻿using Application.Attachments.Commands.Create;
using Application.Attachments.Commands.Delete;
using Application.Common;
using Application.Common.Constants;
using Application.Common.Enums;
using Application.Shipments.Commands.Create;
using IntegrationTests.Common;
using IntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Tests.Commands.Attachments;

[Collection("TestSetupCollection")]
public class DeleteAttachmentTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public DeleteAttachmentTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldDeleteAttachment()
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
        Result<CreateAttachmentResult> createAttachmentResult =
            await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateAttachmentCommand, Result<CreateAttachmentResult>>(createAttachmentCommand);

        DeleteAttachmentCommand deleteAttachmentCommand =
            new(createAttachmentResult.Data.AttachmentId, createAttachmentResult.Data.ShipmentId, "ABCD@equinor.com", "ABCD");
        Result<bool> deleteAttachmentResult = await _testSetupFixture.
                                                        CommandDispatcher.
                                                        DispatchAsync<DeleteAttachmentCommand, Result<bool>>(deleteAttachmentCommand);

        Assert.True(deleteAttachmentResult.Status == ResultStatusConstants.Success);
        Assert.True(deleteAttachmentResult.Data == true);
        Assert.True(createAttachmentResult.Errors is null);
    }

    [Fact]
    public async Task DispatchShouldNotDeleteAttachmentReturnsAttachmentNotFound()
    {
        DeleteAttachmentCommand deleteAttachmentCommand = new(Guid.NewGuid(), Guid.NewGuid(), "ABCD@equinor.com", "ABCD");
        Result<bool> result = await _testSetupFixture.
                                                        CommandDispatcher.
                                                        DispatchAsync<DeleteAttachmentCommand, Result<bool>>(deleteAttachmentCommand);

        Assert.True(result.Status == ResultStatusConstants.NotFound);
        Assert.True(result.Data == false);
        Assert.True(result.Errors is not null);
        Assert.Contains(ShipmentValidationErrors.ShipmentNotFoundText, result.Errors);
    }
}