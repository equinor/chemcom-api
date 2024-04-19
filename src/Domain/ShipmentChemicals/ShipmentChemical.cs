using Domain.Chemicals;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.ShipmentChemicals;

public class ShipmentChemical
{
    public ShipmentChemical(Guid chemicalId,
        Guid shipmentId,
        double amount,
        string measureUnit,
        double calculatedBiocides,
        double calculatedNitrogen,
        double calculatedToc,
        double calculatedBiocidesUnrinsed,
        double calculatedNitrogenUnrinsed,
        double calculatedTocUnrinsed,
        string updatedBy,
        string updatedbyName)
    {
        Id = Guid.NewGuid();
        ChemicalId = chemicalId;
        ShipmentId = shipmentId;
        Amount = amount;
        MeasureUnit = measureUnit;
        CalculatedBiocides = calculatedBiocides;
        CalculatedNitrogen = calculatedNitrogen;
        CalculatedToc = calculatedToc;
        CalculatedBiocidesUnrinsed = calculatedBiocidesUnrinsed;
        CalculatedNitrogenUnrinsed = calculatedNitrogenUnrinsed;
        CalculatedTocUnrinsed = calculatedTocUnrinsed;
        Updated = DateTime.Now;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedbyName;
    }
    public Guid Id { get; private set; }

    public Guid ShipmentId { get; private set; }

    public Guid ChemicalId { get; private set; }

    public string MeasureUnit { get; set; }

    public double Amount { get; set; }

    public double CalculatedWeight { get; set; }

    public double CalculatedToc { get; set; }

    public double CalculatedNitrogen { get; set; }

    public DateTime Updated { get; set; }

    public string UpdatedBy { get; set; }

    public string UpdatedByName { get; set; }

    public double CalculatedBiocides { get; set; }

    public double CalculatedBiocidesUnrinsed { get; set; }

    public double CalculatedNitrogenUnrinsed { get; set; }

    public double CalculatedTocUnrinsed { get; set; }

    public double CalculatedWeightUnrinsed { get; set; }

    public Chemical Chemical { get; set; }

    public Shipment Shipment { get; set; }

    public void Update(double amount,
        string measureUnit,
        double calculatedBiocides,
        double calculatedNitrogen,
        double calculatedToc,
        double calculatedBiocidesUnrinsed,
        double calculatedNitrogenUnrinsed,
        double calculatedTocUnrinsed,
         string updatedBy,
        string updatedbyName)
    {
        Amount = amount;
        MeasureUnit = measureUnit;
        CalculatedBiocides = calculatedBiocides;
        CalculatedNitrogen = calculatedNitrogen;
        CalculatedToc = calculatedToc;
        CalculatedBiocidesUnrinsed = calculatedBiocidesUnrinsed;
        CalculatedNitrogenUnrinsed = calculatedNitrogenUnrinsed;
        CalculatedTocUnrinsed = calculatedTocUnrinsed;
        Updated = DateTime.Now;
        UpdatedBy = updatedBy;
        UpdatedByName = updatedbyName;
    }

    private ShipmentChemical()
    { }
}
