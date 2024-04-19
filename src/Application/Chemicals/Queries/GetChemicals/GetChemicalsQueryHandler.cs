using Application.Common;
using Application.Common.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Queries.GetChemicals;

public class GetChemicalsQueryHandler : IQueryHandler<GetChemicalsQuery, Result<GetChemicalsQueryResult>>
{
    private readonly IChemicalsRepository _chemicalsRepository;

    public GetChemicalsQueryHandler(IChemicalsRepository chemicalsRepository)
    {
        _chemicalsRepository = chemicalsRepository;
    }

    public async Task<Result<GetChemicalsQueryResult>> ExecuteAsync(GetChemicalsQuery query)
    {
        var chemicals = await _chemicalsRepository.GetChemicalsAsync(query.ExcludeActive, query.ExcludeDisabled, query.ExcludeProposed, query.ExcludeNotProposed, query.ForInstallation);

        if (chemicals is null)
        {
            return Result<GetChemicalsQueryResult>.NotFound(new List<string> { "Chemicals not found" });
        }

        GetChemicalsQueryResult result = GetChemicalsQueryResult.Map(chemicals);
        return Result<GetChemicalsQueryResult>.Success(result);
    }
}

