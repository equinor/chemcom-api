using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Queries.GetShipmentChemicalsByShipmentIdCommand;

public sealed record GetShipmentChemicalsByShipmentIdResult
{
    public int ResultsCount { get; set; }
    public List<ShipmentChemicalResult> ShipmentChemicals { get; set; }
}

public sealed record ShipmentChemicalResult
{
    public Guid Id { get; set; }
    public Guid ChemicalId { get; set; }
    public Guid ShipmentId { get; set; }
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
    public string UpdatedByName { get; set; }
    public string UpdatedBy { get; set; }
}
