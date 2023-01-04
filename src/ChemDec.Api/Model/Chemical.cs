using Db = ChemDec.Api.Datamodel;
using AutoMapper;
using System;
using System.Collections.Generic;

namespace ChemDec.Api.Model
{
    public class Chemical
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

        public Reference ProposedByInstallation { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }

    }

    public class ChemicalReference : Reference
    {
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

    }

    public class ChemicalResponse
    {
        public int Total { get; set; }
        public int Skipped { get; set; }
        public int Count => Chemicals.Count;
        public List<Chemical> Chemicals { get; set; }
    }

    public class ChemicalProfile : Profile
    {
        public ChemicalProfile()
        {

            CreateMap<Chemical, Db.Chemical>()
                .ForMember(d => d.ProposedByInstallation, s => s.Ignore());
            CreateMap<Db.Installation, Reference>();

            CreateMap<Db.Chemical, Chemical>();
            CreateMap<Db.Chemical, ChemicalReference>();

        }
    }

   
   

    

   
}
