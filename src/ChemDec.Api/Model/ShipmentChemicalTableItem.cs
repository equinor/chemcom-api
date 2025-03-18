using System;

namespace ChemDec.Api.Model
{
    public class ShipmentChemicalTableItem
    {
        public string ChemicalName { get; set; }
        public double Weight { get; set; }
        public string Description { get; set; }
        public string ShipmentTitle { get; set; }
        public DateTime PlannedExecutionFromDate { get; set; }
        public DateTime PlannedExecutionToDate { get; set; }
        public string FromInstallation { get; set; }

        public double TocWeight { get; set; }
        public double NitrogenWeight { get; set; }
        public double BiocideWeight { get; set; }
        public double Density { get; set; }
        public double Amount { get; set; }
        public string HazardClass { get; set; }//green,yellow,red,black

        public string MeasureUnitDefault { get; set; } //kg,l,tonn,m3
        public string MeasureUnit { get; set; }
        public bool FollowOilPhaseDefault { get; set; }
        public bool FollowWaterPhaseDefault { get; set; }
        public double  Water { get; set; }

        //public DateTime FromDate { get; set; }
        //public DateTime ToDate { get; set; }

        //public ShipmentChemicalTableItem(Datamodel.Chemical chemical, string senderName, 
        //    double sumWeight, double sumTocWeight, double sumNitrogenWeight, double sumBiocideWeight)
        //{
        //    ChemicalName = chemical.Name;
        //    Weight = sumWeight;
        //    Description = chemical.Description;
        //    FromInstallation = senderName;

        //    TocWeight = sumTocWeight;
        //    NitrogenWeight = sumNitrogenWeight;
        //    BiocideWeight = sumBiocideWeight;
        //    Density = chemical.Density;
        //    HazardClass = chemical.HazardClass;

        //    MeasureUnitDefault = chemical.MeasureUnitDefault;
        //    FollowOilPhaseDefault = chemical.FollowOilPhaseDefault;
        //    FollowWaterPhaseDefault = chemical.FollowWaterPhaseDefault;
        //}

        //public ShipmentChemicalTableItem() { }

        //public ShipmentChemicalTableItem(Datamodel.ShipmentChemical sc, Datamodel.Chemical c, string senderName,
        //    DateTime fromDate, DateTime toDate)
        //{
        //    Chemical = c;
        //    //ChemicalName = c.Name;
        //    Weight = sc.CalculatedWeight; //not sure where to get this ? ask Lotte
        //    //Description = c.Description;
        //    FromInstallation = senderName;

        //    TocWeight = sc.CalculatedToc;
        //    NitrogenWeight = sc.CalculatedNitrogen;
        //    BiocideWeight = sc.CalculatedBiocides;
        //    //Density = c.Density;
        //    //HazardClass = c.HazardClass;

        //    //MeasureUnitDefault = c.MeasureUnitDefault;
        //    //FollowOilPhaseDefault = c.FollowOilPhaseDefault;
        //    //FollowWaterPhaseDefault = c.FollowWaterPhaseDefault;

        //    FromDate = fromDate;
        //    ToDate = toDate;
        //}

    }
}
