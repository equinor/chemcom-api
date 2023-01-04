using ChemDec.Api.Datamodel.Interfaces;
using System;

namespace ChemDec.Api.Datamodel
{
    public class FieldChange : IAudit
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



}
