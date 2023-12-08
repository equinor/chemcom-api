using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Notifications;

public class Notification
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
