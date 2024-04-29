using System;

namespace ChemDec.Api.Endpoints.Shipments.Submit
{
    public class SubmitShipmentRequest
    {
        public bool? AvailableForDailyContact { get; internal set; }
        public bool HeightenedLra { get; internal set; }
        public bool TakePrecaution { get; internal set; }
        public string Precautions { get; internal set; }
        public double? Pb210 { get; internal set; }
        public double? Ra226 { get; internal set; }
        public double? Ra228 { get; internal set; }
    }
}