using ChemDec.Api.Datamodel.Interfaces;
using System;
using System.Collections.Generic;

namespace ChemDec.Api.Datamodel
{
    public class LogEntry : IAudit
    {
        public Guid Id { get; set; }
        public Guid ShipmentId { get; set; }
        public Shipment Shipment { get; set; }
        public string EventType { get; set; }
        public string Description { get; set; }
        public Guid InstallationId { get; set; }
        public Installation Installation { get; set; }
        public ICollection<FieldChange> FieldChanges { get; set; }
        public DateTime Updated { get; set; }
        public string UpdatedBy { get; set; }
        public string UpdatedByName { get; set; }
    }



}
