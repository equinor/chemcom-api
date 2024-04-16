using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using Db = ChemDec.Api.Datamodel;

namespace ChemDec.Api.Model
{
    public class Installation
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string TimeZone { get; set; }
        public string InstallationType { get; set; } //Platform, Plant
        public string Terms { get; set; } //Specific terms for this plant
        public string Contact { get; set; } //Contact info for this plant
        public double TocCapacity { get; set; }
        public double WaterCapacity { get; set; }
        public double NitrogenCapacity { get; set; }
        public double Toc { get; set; }
        public double Water { get; set; }
        public double Nitrogen { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public Reference ShipsTo { get; set; }
      //  public IEnumerable<Reference> GetsShipmentsFrom { get; set; }
        public IEnumerable<Reference> ShipsToPlant { get; set; }
        public IEnumerable<Reference> GetsShipmentsFromInstallation { get; set; }


    }

    public class InstallationResponse
    {
        public int Total { get; set; }
        public int Skipped { get; set; }
        public int Count => Installations.Count;
        public List<Installation> Installations { get; set; }
    }

    public class PlantReference: Reference {
        public string Terms { get; set; }
        public string Contact { get; set; }
        public IEnumerable<string> ContactList { get { return Contact?.Split(new[] {','},StringSplitOptions.RemoveEmptyEntries); } }
    }

    public class InstallationReference
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string TimeZone { get; set; }

        public string InstallationType { get; set; }
        public double TocCapacity { get; set; }
        public double WaterCapacity { get; set; }
        public double NitrogenCapacity { get; set; }

        public double Toc { get; set; }
        public double Water { get; set; }
        public double Nitrogen { get; set; }

        public IEnumerable<PlantReference> ShipsTo { get; set; }
    }

    public class InstallationProfile : Profile
    {
        public InstallationProfile()
        {

            CreateMap<Installation, Db.Installation>();
            CreateMap<Db.Installation, Installation>()
                .ForMember(d => d.ShipsToPlant, s => s.MapFrom(m => m.ShipsToPlant.Select(ss => ss.Plant)))
                .ForMember(d => d.GetsShipmentsFromInstallation, s => s.MapFrom(m => m.GetsShipmentsFromInstallation.Select(ss => ss.Installation)));
            CreateMap<Db.Installation, Reference>();
            CreateMap<Db.Installation, InstallationReference>()
                .ForMember(d => d.ShipsTo, s => s.MapFrom(m => m.ShipsToPlant.Select(ss => ss.Plant)));
            CreateMap<Db.Installation, PlantReference>();  
        }
    }
}
