using Application.Common;
using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Chemicals.Commands.AddShipmentChemical;

public sealed class AddShipmentChemicalsCommand : ICommand
{
    public Guid ShipmentId { get; set; }
    public User User { get; set; }
    public AddShipmentChemicalsCommand()
    {
        ShipmentChemicalItems = new List<ShipmentChemicalItem>();
    }

    public List<ShipmentChemicalItem> ShipmentChemicalItems { get; set; }
}

public sealed record ShipmentChemicalItem
{
    public Guid ChemicalId { get; set; }
    public string MeasureUnit { get; set; }
    public double Amount { get; set; }
    public double CalculatedWeightUnrinsed { get; set; } //in kg
    public double CalculatedTocUnrinsed { get; set; } //in kg
    public double CalculatedNitrogenUnrinsed { get; set; } //in kg
    public double CalculatedBiocidesUnrinsed { get; set; } //in kg
    public double CalculatedWeight { get; set; } //in kg
    public double CalculatedToc { get; set; } //in kg
    public double CalculatedNitrogen { get; set; } //in kg
    public double CalculatedBiocides { get; set; } //in kg    
}
