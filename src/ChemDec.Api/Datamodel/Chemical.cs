﻿using ChemDec.Api.Datamodel.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChemDec.Api.Datamodel
{
    public class Chemical : IAudit
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

        [ForeignKey("ProposedByInstallationId")]
        public Installation ProposedByInstallation { get; set; }
        public Guid? ProposedByInstallationId { get; set; }

        public ICollection<ShipmentChemical> Shipments { get; set; }

        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }


    }





}
