using Application.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.Approve;

public sealed record ApproveChemicalCommand : ICommand
{
    public Guid ChemicalId { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }

    public ApproveChemicalCommand(Guid chemicalId, string updatedBy, string updatedByName)
    {
        ChemicalId = chemicalId;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedByName;        
    }
}
