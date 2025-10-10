namespace ChemDec.Api.Model.dto
{
    public class ShipmentChemicalExportTableDto
    {
        public string ChemicalName { get; set; }
        public string Weight { get; set; }
        public string Description { get; set; }
        public string ShipmentTitle { get; set; }
        public string PlannedExecutionFromDate { get; set; }
        public string PlannedExecutionToDate { get; set; }
        public string FromInstallation { get; set; }

        public string TocWeight { get; set; }
        public string NitrogenWeight { get; set; }
        public string BiocideWeight { get; set; }
        public double Density { get; set; }
        public double Amount { get; set; }
        public string HazardClass { get; set; }

        public string MeasureUnitDefault { get; set; } 
        public string MeasureUnit { get; set; }
        public bool FollowOilPhaseDefault { get; set; }
        public bool FollowWaterPhaseDefault { get; set; }
        public double Water { get; set; }
    }
}
