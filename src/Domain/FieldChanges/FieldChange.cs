using Domain.LogEntries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.FieldChanges;

public class FieldChange
{
    public Guid Id { get; set; }
    public Guid LogId { get; set; }
    public LogEntry Log { get; set; }
    public string FromField { get; set; }
    public string FromValue { get; set; }
    public string ToField { get; set; }
    public string ToValue { get; set; }
    public DateTime Updated { get; set; }
    public string UpdatedBy { get; set; }
    public string UpdatedByName { get; set; }
}
