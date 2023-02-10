namespace ChemDec.Api.Model
{
    public class ReservoirData
    {
        public double Toc { get; set; }
        public double Nitrogen { get; set; }
        public double Water { get; set; }
        public double TocApproved { get; set; }
        public double NitrogenApproved { get; set; }
        public double WaterApproved { get; set; }
        public double TocPending { get; set; }
        public double NitrogenPending { get; set; }
        public double WaterPending { get; set; }
    }
}
