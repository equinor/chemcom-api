using Db = ChemDec.Api.Datamodel;
using System;
using AutoMapper;

namespace ChemDec.Api.Model
{
    public class ShipmentChemical { 

        public Guid Id { get; set; }
        public ChemicalReference Chemical { get; set; }

        public string MeasureUnit { get; set; }
        public double Amount { get; set; }

        public double CalculatedWeightUnrinsed { get; set; } //in kg
        public double CalculatedTocUnrinsed { get; set; } //in kg
        public double CalculatedNitrogenUnrinsed { get; set; } //in kg
        public double CalculatedBiocidesUnrinsed { get; set; } //in kg


        public double CalculatedWeight { get; set; } //in kg
        public double CalculatedToc { get; set; } //in kg
        public double CalculatedNitrogen { get; set; } //in kg
        public double CalculatedBiocides { get; set; } //in kg 

        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }

    }
    public class ShipmentChemicalProfile : Profile
    {
        public ShipmentChemicalProfile()
        {

            CreateMap<ShipmentChemical, Db.ShipmentChemical>()
                .ForMember(dest => dest.Chemical, m => m.Ignore())
                .ForMember(dest => dest.ChemicalId, m => m.MapFrom(i => i.Chemical.Id))
                ;
            ;
            CreateMap<Db.ShipmentChemical, ShipmentChemical>();

        }
    }


}
