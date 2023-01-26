using System.Collections.Generic;

namespace ChemDec.Api.Model
{
    public class ChemicalBase
    {
        public string Name { get; set; }
        public string Stack { get; set; }
        public string Color { get; set; }
        public double YAxis { get; set; }
        public IList<ChemicalData> Data { get; set; }
        public double MaxPointWidth { get; set; }
        public double? Total { get; set; }
    }
}
