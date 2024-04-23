using Application.Common;
using Application.Common.Repositories;
using Domain.Chemicals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.Create;

public sealed class CreateChemicalCommandHandler : ICommandHandler<CreateChemicalCommand, Result<CreateChemicalResult>>
{
    private readonly IChemicalsRepository _chemicalsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateChemicalCommandHandler(IChemicalsRepository chemicalsRepository, IUnitOfWork unitOfWork)
    {
        _chemicalsRepository = chemicalsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<CreateChemicalResult>> HandleAsync(CreateChemicalCommand command, CancellationToken cancellationToken = default)
    {
        List<string> errors = new();

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            errors.Add("Chemical name must be set");
        }

        if (string.IsNullOrWhiteSpace(command.Description))
        {
            errors.Add("Chemical description must be set");
        }

        if (command.Name.Contains(";"))
        {
            errors.Add("Chemical name cannot contain semicolons");
        }

        if (command.Description.Contains(";"))
        {
            errors.Add("Chemical description cannot contain semicolons");
        }

        bool chemicalExists = await _chemicalsRepository.ExistsAsync(command.Name.Trim(), cancellationToken);

        if (chemicalExists)
        {
            errors.Add($"Chemical with the name {command.Name} already exists");
        }

        if (errors.Any())
        {
            return Result<CreateChemicalResult>.Failed(errors);
        }

        Chemical chemical = CreateChemicalCommand.Map(command);
        chemical.SetNewId();
        await _chemicalsRepository.InsertAsync(chemical, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        CreateChemicalResult result = CreateChemicalResult.Map(chemical);
        return Result<CreateChemicalResult>.Success(result);
    }
}
