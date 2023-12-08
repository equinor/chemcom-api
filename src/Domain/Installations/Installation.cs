using Domain.Chemicals;
using Domain.InstallationPlants;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Installations;

public class Installation
{
    public Guid Id { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string TimeZone { get; set; }
    public string InstallationType { get; set; } //Platform, Plant
    public string Terms { get; set; } //Specific terms for this plant
    public string Contact { get; set; } //Contact info for this plant
    public double TocCapacity { get; set; }
    public double WaterCapacity { get; set; }
    public double NitrogenCapacity { get; set; }
    public double Toc { get; set; }
    public double Water { get; set; }
    public double Nitrogen { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
    public Guid? ShipsToId { get; set; }
    public Installation ShipsTo { get; set; }
    public ICollection<Chemical> Chemicals { get; set; }
    public ICollection<InstallationPlant> ShipsToPlant { get; set; }
    public ICollection<Installation> GetsShipmentsFrom { get; set; }
    public ICollection<InstallationPlant> GetsShipmentsFromInstallation { get; set; }
    public ICollection<Shipment> SentShipments { get; set; }
    public ICollection<Shipment> ReceivedShipments { get; set; }
}
