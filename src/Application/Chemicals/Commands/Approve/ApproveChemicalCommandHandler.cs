using Application.Common;
using Application.Common.Repositories;
using Domain.Chemicals;

namespace Application.Chemicals.Commands.Approve;

public sealed class ApproveChemicalCommandHandler : ICommandHandler<ApproveChemicalCommand, Result<bool>>
{
    private readonly IChemicalsRepository _chemicalsRepository;
    private readonly IUnitOfWork _unitOfWork;
    public ApproveChemicalCommandHandler(IChemicalsRepository chemicalsRepository, IUnitOfWork unitOfWork)
    {
        _chemicalsRepository = chemicalsRepository;
        _unitOfWork = unitOfWork;
    }
    public async Task<Result<bool>> HandleAsync(ApproveChemicalCommand command)
    {
        Chemical chemical = await _chemicalsRepository.GetByIdAsync(command.ChemicalId);

        if (chemical is null)
        {
            return Result<bool>.NotFound(new List<string> { "Chemical not found" });
        }

        chemical.Approve();
        _chemicalsRepository.Update(chemical);
        await _unitOfWork.CommitChangesAsync();
        return Result<bool>.Success(true);
    }
}
