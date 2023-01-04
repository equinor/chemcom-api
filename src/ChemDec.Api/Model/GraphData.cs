using System.Collections.Generic;

namespace ChemDec.Api.Model
{
    public class GraphData {
        public List<GraphLabel> ShipmentDates { get; set; }
        public List<GraphItem> Chemicals { get; set; }
    }
    public class GraphLabel
    {
        public string Label { get; set; }
        public bool HasBiocides { get; set; }
    }
    public class DataItem
    {
        public double Y { get; set; }
        public bool HasBiocides { get; set; }

        public string Metric { get; set; } 
    }
    public class GraphItem
    {
        public string Name { get; set; }
        public List<DataItem> Data { get; set; }
        public string Stack { get; set; }
        public string Color { get; set; }
        public int YAxis { get; internal set; }

        public int MaxPointWidth { get; set; }
    }

    public class GraphFlat
    {
        public int Year { get; set; }
        public int Month { get; set; }
        public int Day { get; set; }
        public int Hour { get; set; } = -1;
        public string Date => Year == 0 ? "Total" : string.Join('.',new[] {Day,Month,Year}) + (Hour>=0 ? ":"+Hour : string.Empty);
        public double Toc { get; set; }
        public double Nitrogen { get; set; }
        public double Water { get; set; }
        public double Biocides { get; set; }
        public double TocPending { get; set; }
        public double NitrogenPending { get; set; }
        public double PendingWater { get; set; }
        public double BiocidesPending { get; set; }
    }



}
