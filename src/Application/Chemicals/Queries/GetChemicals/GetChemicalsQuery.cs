using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Queries.GetChemicals;

public sealed record GetChemicalsQuery : IQuery
{
    public GetChemicalsQuery(bool excludeActive,
                             bool excludeDisabled,
                             bool excludeProposed,
                             bool excludeNotProposed,
                             Guid? forInstallation)
    {
        ExcludeActive = ExcludeActive;
        ExcludeDisabled = ExcludeDisabled;
        ExcludeProposed = ExcludeProposed;
        ExcludeNotProposed = ExcludeNotProposed;
        ForInstallation = ForInstallation;
    }
    public bool ExcludeActive { get; set; }
    public bool ExcludeDisabled { get; set; }
    public bool ExcludeProposed { get; set; }
    public bool ExcludeNotProposed { get; set; }
    public Guid? ForInstallation { get; set; }
}
