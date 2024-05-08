using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.EmailNotifications;

public class EmailNotification
{
    public Guid Id { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }
    public string Recipients { get; set; }
    public DateTime? SentAt { get; set; }
    public int EmailNotificationType { get; set; }
    public bool IsSent { get; set; }
    public string ErrorMessage { get; set; }
}
