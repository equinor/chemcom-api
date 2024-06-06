using System;

namespace ChemDec.Api.Endpoints.Shipments.Submit;

public sealed record SubmitShipmentRequest
{
    public bool? AvailableForDailyContact { get; set; }
    public bool HeightenedLra { get; set; }
    public bool TakePrecaution { get; set; }
    public string Precautions { get; set; }
    public double? Pb210 { get; set; }
    public double? Ra226 { get; set; }
    public double? Ra228 { get; set; }
}