﻿using System;

namespace ChemDec.Api.Endpoints.Shipments.Chemicals.Add;

public sealed record AddShipmentChemicalRequest
{
    public Guid ChemicalId { get; set; }
    public string MeasureUnit { get; set; }
    public double Amount { get; set; }
    public double CalculatedWeightUnrinsed { get; set; }
    public double CalculatedTocUnrinsed { get; set; }
    public double CalculatedNitrogenUnrinsed { get; set; }
    public double CalculatedBiocidesUnrinsed { get; set; }
    public double CalculatedWeight { get; set; }
    public double CalculatedToc { get; set; }
    public double CalculatedNitrogen { get; set; }
    public double CalculatedBiocides { get; set; }
}
