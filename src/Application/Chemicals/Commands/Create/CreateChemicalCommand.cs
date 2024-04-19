using Application.Common;
using Domain.Chemicals;
using Domain.Installations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.Create;

public sealed record CreateChemicalCommand : ICommand
{
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
    public string ProposedBy { get; set; }
    public string ProposedByName { get; set; }
    public string ProposedByEmail { get; set; }
    public bool Disabled { get; set; }
    public Guid? ProposedByInstallationId { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }

    public static Chemical Map(CreateChemicalCommand command)
    {
        return new Chemical
        {
            Name = command.Name,
            Description = command.Description,
            TocWeight = command.TocWeight,
            NitrogenWeight = command.NitrogenWeight,
            BiocideWeight = command.BiocideWeight,
            Density = command.Density,
            HazardClass = command.HazardClass,
            MeasureUnitDefault = command.MeasureUnitDefault,
            FollowOilPhaseDefault = command.FollowOilPhaseDefault,
            FollowWaterPhaseDefault = command.FollowWaterPhaseDefault,
            Tentative = command.Tentative,
            Proposed = DateTime.Now,
            ProposedBy = command.ProposedBy,
            ProposedByName = command.ProposedByName,
            ProposedByEmail = command.ProposedByEmail,
            Disabled = command.Disabled,
            ProposedByInstallationId = command.ProposedByInstallationId,
            Updated = DateTime.Now,
            UpdatedBy = command.UpdatedBy,
            UpdatedByName = command.UpdatedByName
        };
    }
}
