﻿using Domain.Common;
using Domain.Installations;
using Domain.ShipmentChemicals;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Chemicals;

public class Chemical : IAuditable
{
    public Guid Id { get; private set; }
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
    public Installation ProposedByInstallation { get; set; }
    public Guid? ProposedByInstallationId { get; set; }

    //public ICollection<ShipmentChemical> ShipmentChemicals { get; set; }

    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }

    public void Approve()
    {
        Tentative = false;
    }

    public void SetNewId()
    {
        Id = Guid.NewGuid();
    }   
}
