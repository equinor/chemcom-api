using ChemDec.Api.Datamodel.Interfaces;
using System;
using System.Collections.Generic;

namespace ChemDec.Api.Datamodel
{
    public class Installation : IAudit
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
        public ICollection<InstallationPlant> ShipsToPlant { get; set; }
        public ICollection<Installation> GetsShipmentsFrom { get; set; }
        public ICollection<InstallationPlant> GetsShipmentsFromInstallation { get; set; }


        public ICollection<Shipment> SendtShipments { get; set; }
        public ICollection<Shipment> ReceivedShipments { get; set; }
    }

    public class InstallationPlant
    {
        public Guid InstallationId { get; set; }
        public Installation Installation { get; set; }

        public Guid PlantId { get; set; }
        public Installation Plant { get; set; }
    }




}
