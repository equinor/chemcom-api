using Domain.Chemicals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.Create;

public sealed record CreateChemicalResult
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public double TocWeight { get; set; }
    public double NitrogenWeight { get; set; }
    public double BiocideWeight { get; set; }
    public double Density { get; set; }
    public string HazardClass { get; set; } //green,yellow,red,black
    public string MeasureUnitDefault { get; set; } //kg,l,tonn,m3
    public bool FollowOilPhaseDefault { get; set; }
    public bool FollowWaterPhaseDefault { get; set; }
    public bool Tentative { get; set; }
    public DateTime? Proposed { get; set; }
    public string ProposedBy { get; set; }
    public string ProposedByName { get; set; }
    public string ProposedByEmail { get; set; }
    public bool Disabled { get; set; }
    public Guid? ProposedByInstallationId { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }

    public static CreateChemicalResult Map(Chemical chemical)
    {
        return new CreateChemicalResult
        {
            Id = chemical.Id,
            Name = chemical.Name,
            Description = chemical.Description,
            TocWeight = chemical.TocWeight,
            NitrogenWeight = chemical.NitrogenWeight,
            BiocideWeight = chemical.BiocideWeight,
            Density = chemical.Density,
            HazardClass = chemical.HazardClass,
            MeasureUnitDefault = chemical.MeasureUnitDefault,
            FollowOilPhaseDefault = chemical.FollowOilPhaseDefault,
            FollowWaterPhaseDefault = chemical.FollowWaterPhaseDefault,
            Tentative = chemical.Tentative,
            Proposed = chemical.Proposed,
            ProposedBy = chemical.ProposedBy,
            ProposedByName = chemical.ProposedByName,
            ProposedByEmail = chemical.ProposedByEmail,
            Disabled = chemical.Disabled,
            ProposedByInstallationId = chemical.ProposedByInstallationId,
            Updated = chemical.Updated,
            UpdatedBy = chemical.UpdatedBy,
            UpdatedByName = chemical.UpdatedByName
        };
    }
}
