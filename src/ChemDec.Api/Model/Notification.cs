using System;
using System.Collections.Generic;
using System.Linq;
using ChemDec.Api.Datamodel;

namespace ChemDec.Api.Model
{
    public class NotificationItem
    {
        internal Notification _notification;
        public NotificationItem(Notification notification)
        {
            if (notification == null)
            {
                _notification = new Notification();
            }
            else
            {
                _notification = notification;
            }

        }
        public NotificationItem()
        {
            _notification = new Notification();
        }
        public Guid Id
        {
            get { return _notification.Id; }
            set { _notification.Id = value; }
        }

        public DateTime FromDate
        {
            get { return DateTime.SpecifyKind(_notification.FromDate, DateTimeKind.Utc); }
            
            set { _notification.FromDate = DateTime.SpecifyKind(value, DateTimeKind.Utc); }
        }
        
        //To date is not required and should be nullable
        public DateTime? ToDate
        {
            get { return _notification.ToDate != null ? DateTime.SpecifyKind(_notification.ToDate.Value, DateTimeKind.Utc) : _notification.ToDate; }
            set { _notification.ToDate = (value != null && value.Value != null) ? DateTime.SpecifyKind(value.Value, DateTimeKind.Utc) : value; }
        }
        public NotificationType NotificationType
        {
            get { return _notification.NotificationType; }
            set { _notification.NotificationType = value; }
        }

        public string Message
        {
            get { return _notification?.Message; }
            set { _notification.Message = value; }
        }
        public DateTime? Created
        {
            get { return _notification.Created; }
        }
        public string CreatedBy
        {
            get { return _notification.CreatedBy; }
            set { _notification.CreatedBy = value; }
        }

        public DateTime? Updated
        {
            get { return _notification.Updated; }
        }
        public string UpdatedBy
        {
            get { return _notification.UpdatedBy; }
            set { _notification.UpdatedBy = value; }
        }
    }

    public class NotificationList
    {
        private readonly List<Notification> _notifications;
        public NotificationList(List<Notification> notifications)
        {
            _notifications = notifications;
        }

        public IEnumerable<NotificationItem> Notifications
        {
            get
            {
                return _notifications.Select(i => new NotificationItem(i));
            }
        }
    }
}

