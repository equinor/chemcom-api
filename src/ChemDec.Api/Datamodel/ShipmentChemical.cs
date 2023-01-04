using ChemDec.Api.Datamodel.Interfaces;
using System;

namespace ChemDec.Api.Datamodel
{
    public class ShipmentChemical : IAudit
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Shipment Shipment { get; set; }

        public Guid ChemicalId { get; set; }
        public Chemical Chemical { get; set; }

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
        public string UpdatedByName { get; set; }
        
    }





}
