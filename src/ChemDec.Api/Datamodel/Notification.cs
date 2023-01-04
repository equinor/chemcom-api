using System;

namespace ChemDec.Api.Datamodel
{
    public class Notification : UpdatableBaseTable
    { 
        public DateTime FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public NotificationType NotificationType { get; set; }
        public string Message { get; set; }
    }

    public enum NotificationType
    {
        Warning,
        Positive,
        Info,
        Bcc,
    };
}





