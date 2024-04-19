using Application.Chemicals.Commands.Create;
using Application.Common;
using IntegrationTests.Fixtures;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Tests.Commands.Chemicals;

[Collection("TestSetupCollection")]
public sealed class CreateChemicalCommandTests
{
    private readonly TestSetupFixture _testSetupFixture;
    public CreateChemicalCommandTests(TestSetupFixture testSetupFixture)
    {
        _testSetupFixture = testSetupFixture;
    }

    [Fact]
    public async Task DispatchShouldCreateChemical()
    {
        CreateChemicalCommand command = new CreateChemicalCommand
        {
            Name = "Test Chemical",
            Description = "Test Description",
            Tentative = true,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<CreateChemicalResult> result = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(command);
        Assert.True(result.Status == ResultStatusConstants.Success);
        Assert.True(result.Data is not null);
        Assert.True(result.Errors is null);
    }
}
