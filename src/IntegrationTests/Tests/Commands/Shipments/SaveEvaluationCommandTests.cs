using Application.Common;
using Application.Shipments.Commands.Create;
using Application.Shipments.Commands.Evaluate;
using Domain.Users;
using IntegrationTests.Common;
using IntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Tests.Commands.Shipments;

[Collection("TestSetupCollection")]
public sealed class SaveEvaluationCommandTests
{
    private readonly TestSetupFixture _testSetupFixture;

    public SaveEvaluationCommandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldUpdateValuationValues()
    {
        User user = await _testSetupFixture.UserProvider.GetUserAsync(_testSetupFixture.ClaimsPrincipal);
        CreateShipmentCommand createShipmentCommand = new CreateShipmentCommand()
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

        Result<CreateShipmentResult> createResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateShipmentCommand, Result<CreateShipmentResult>>(createShipmentCommand);

        SaveEvaluationCommand saveEvaluationCommand = new SaveEvaluationCommand()
        {
            ShipmentId = createResult.Data.Id,
            EvalAmountOk = true,
            EvalBiocidesOk = true,
            EvalCapacityOk = true,
            EvalContaminationRisk = true,
            EvalEnvImpact = "Test",
            EvalComments = "Test comments",
            User = user
        };

        Result<bool> saveEvaluationResult = await _testSetupFixture.CommandDispatcher.DispatchAsync<SaveEvaluationCommand, Result<bool>>(saveEvaluationCommand);

        Assert.True(saveEvaluationResult.Status == ResultStatusConstants.Success);
        Assert.True(saveEvaluationResult.Data);
        Assert.True(saveEvaluationResult.Errors is null);
    }
}
