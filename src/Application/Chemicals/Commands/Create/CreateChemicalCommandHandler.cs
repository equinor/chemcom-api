using Application.Common;
using Application.Common.Constants;
using Application.Common.Repositories;
using Domain.Chemicals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.Create;

public sealed class CreateChemicalCommandHandler : ICommandHandler<CreateChemicalCommand, Result<Guid>>
{
    private readonly IChemicalsRepository _chemicalsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public CreateChemicalCommandHandler(IChemicalsRepository chemicalsRepository, IUnitOfWork unitOfWork)
    {
        _chemicalsRepository = chemicalsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<Guid>> HandleAsync(CreateChemicalCommand command, CancellationToken cancellationToken = default)
    {
        List<string> errors = new();

        if (string.IsNullOrWhiteSpace(command.Name))
        {
            errors.Add(ChemicalValidationErrors.ChemicalNameRequiredText);
        }

        if (string.IsNullOrWhiteSpace(command.Description))
        {
            errors.Add(ChemicalValidationErrors.ChemicalDescriptionRequiredText);
        }

        if (errors.Any())
        {
            return Result<Guid>.Failed(errors);
        }

        if (command.Name.Contains(';'))
        {
            errors.Add(ChemicalValidationErrors.ChemicalNameSemicolonNotAllowedText);
        }

        if (command.Description.Contains(';'))
        {
            errors.Add(ChemicalValidationErrors.ChemicalDescriptionSemicolonNotAllowedText);
        }

        bool chemicalExists = await _chemicalsRepository.ExistsAsync(command.Name.Trim(), cancellationToken);

        if (chemicalExists)
        {
            errors.Add(string.Format(ChemicalValidationErrors.ChemicalAlreadyExistsText, command.Name));
        }

        if (errors.Any())
        {
            return Result<Guid>.Failed(errors);
        }

        Chemical chemical = CreateChemicalCommand.Map(command);
        chemical.SetNewId();
        await _chemicalsRepository.InsertAsync(chemical, cancellationToken);
        await _unitOfWork.CommitChangesAsync(cancellationToken);       
        return Result<Guid>.Success(chemical.Id);
    }
}
