using Domain.FieldChanges;
using Domain.Installations;
using Domain.Shipments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.LogEntries;

public class LogEntry
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
