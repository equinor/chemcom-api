using ChemDec.Api.Datamodel.Interfaces;
using System;

namespace ChemDec.Api.Datamodel
{
    public class Attachment : IAudit
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public string Path { get; set; }
        public string Extension { get; set; }
        public string MimeType { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
    }
}
