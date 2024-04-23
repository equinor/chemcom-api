using Application.Chemicals.Commands.Create;
using Application.Common;
using Application.Common.Repositories;
using Domain.Chemicals;
using Domain.ShipmentChemicals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.Update;

public sealed class UpdateChemicalCommandHandler : ICommandHandler<UpdateChemicalCommand, Result<UpdateChemicalResult>>
{
    private readonly IChemicalsRepository _chemicalsRepository;
    private readonly IUnitOfWork _unitOfWork;

    public UpdateChemicalCommandHandler(IChemicalsRepository chemicalsRepository, IUnitOfWork unitOfWork)
    {
        _chemicalsRepository = chemicalsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<UpdateChemicalResult>> HandleAsync(UpdateChemicalCommand command, CancellationToken cancellationToken = default)
    {
        Chemical chemical = await _chemicalsRepository.GetByIdAsync(command.Id, cancellationToken);

        if (chemical is null)
        {
            return Result<UpdateChemicalResult>.NotFound(new List<string> { "Chemical not found" });
        }

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
            return Result<UpdateChemicalResult>.Failed(errors);
        }

        List<ShipmentChemical> shipmentChemicals = await _chemicalsRepository.GetShipmentChemicalsByChemicalIdAsync(command.Id, cancellationToken);

        //TODO: Claculate chemicals
        foreach (ShipmentChemical item in shipmentChemicals)
        {

        }

        chemical = UpdateChemicalCommand.Map(command);
        _chemicalsRepository.Update(chemical);
        await _unitOfWork.CommitChangesAsync(cancellationToken);
        return Result<UpdateChemicalResult>.Success(UpdateChemicalResult.Map(chemical));
    }
}
