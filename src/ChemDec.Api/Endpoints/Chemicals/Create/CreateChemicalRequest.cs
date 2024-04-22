﻿using System;

namespace ChemDec.Api.Endpoints.Chemicals.Create;

public sealed record CreateChemicalRequest
{
    public string Name { get; set; }
    public string Description { get; set; }
    public double TocWeight { get; set; }
    public double NitrogenWeight { get; set; }
    public double BiocideWeight { get; set; }
    public double Density { get; set; }
    public string HazardClass { get; set; }
    public string MeasureUnitDefault { get; set; }
    public bool FollowOilPhaseDefault { get; set; }
    public bool FollowWaterPhaseDefault { get; set; }
    public bool Tentative { get; set; }
    public string ProposedBy { get; set; }
    public string ProposedByName { get; set; }
    public string ProposedByEmail { get; set; }
    public bool Disabled { get; set; }
    public Guid? ProposedByInstallationId { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
}
