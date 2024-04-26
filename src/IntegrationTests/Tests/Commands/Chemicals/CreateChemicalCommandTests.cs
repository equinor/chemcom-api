using Application.Chemicals.Commands.Create;
using Application.Common;
using Application.Common.Constants;
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


    [Fact]
    public async Task DispatchShouldNotCreateChemicalWithValidationFailingNameAndDescription()
    {
        CreateChemicalCommand command = new CreateChemicalCommand
        {
            Name = "",
            Description = null,
            Tentative = true,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<CreateChemicalResult> result = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(command);
        
        Assert.True(result.Status == ResultStatusConstants.Failed);
        Assert.True(result.Data is null);
        Assert.True(result.Errors is not null);
        Assert.Contains(ChemicalValidationErrors.ChemicalNameRequiredText, result.Errors);
        Assert.Contains(ChemicalValidationErrors.ChemicalDescriptionRequiredText, result.Errors);
    }

    [Fact]
    public async Task DispatchShouldNotCreateChemicalWithValidationFailingSemicolons()
    {
        CreateChemicalCommand command = new CreateChemicalCommand
        {
            Name = "Test ;name",
            Description = "Test description;",
            Tentative = true,
            UpdatedBy = "ABCD@equinor.com",
            UpdatedByName = "ABCD",
            ProposedBy = "ABCD@equinor.com",
            ProposedByName = "ABCD",
            ProposedByEmail = "ABCD@equinor.com"
        };

        Result<CreateChemicalResult> result = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(command);

        Assert.True(result.Status == ResultStatusConstants.Failed);
        Assert.True(result.Data is null);
        Assert.True(result.Errors is not null);
        Assert.Contains(ChemicalValidationErrors.ChemicalNameSemicolonNotAllowedText, result.Errors);
        Assert.Contains(ChemicalValidationErrors.ChemicalDescriptionSemicolonNotAllowedText, result.Errors);
    }

    [Fact]
    public async Task DispatchShouldNotCreateChemicalChemicalAlreadyExists()
    {
        CreateChemicalCommand commandA = new CreateChemicalCommand
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

        Result<CreateChemicalResult> resultA = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(commandA);

        CreateChemicalCommand commandB = new CreateChemicalCommand
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

        Result<CreateChemicalResult> resultB = await _testSetupFixture.CommandDispatcher.DispatchAsync<CreateChemicalCommand, Result<CreateChemicalResult>>(commandA);

        Assert.True(resultB.Status == ResultStatusConstants.Failed);
        Assert.True(resultB.Data is null);
        Assert.True(resultB.Errors is not null);
        Assert.Contains(string.Format(ChemicalValidationErrors.ChemicalAlreadyExistsText, commandB.Name), resultB.Errors);
    }
}
