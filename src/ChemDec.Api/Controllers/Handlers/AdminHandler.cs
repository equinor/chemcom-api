using Db = ChemDec.Api.Datamodel;
using System.Linq;
using AutoMapper;
using ChemDec.Api.Infrastructure.Utils;
using ChemDec.Api.Model;
using AutoMapper.QueryableExtensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;

namespace ChemDec.Api.Controllers.Handlers
{
    public class AdminHandler
    {
        private readonly Db.ChemContext db;
        private readonly IMapper mapper;
        private readonly UserResolver userResolver;

        public AdminHandler(Db.ChemContext db, IMapper mapper, UserResolver userResolver)
        {
            this.db = db;
            this.mapper = mapper;
            this.userResolver = userResolver;
        }

        public NotificationItem GetCurrentNotification()
        {
            //return the notification with the latest fromDate time before utcNow which has a 
            //toDate time that is later than utcNow
            var theTimeNow = DateTime.UtcNow;
            
            var notifications = GetNotifications()
                .Where(n => n.FromDate <= theTimeNow && (n.ToDate == null || n.ToDate >= theTimeNow))
                .OrderByDescending(n => n.FromDate);
            if (notifications.Count() >= 1)
            {
                return notifications.First();
            }
            return null;
        }

        public IQueryable<NotificationItem> GetNotifications()
        {
            return db.Notifications.ProjectTo<NotificationItem>(mapper.ConfigurationProvider);
        }

        public async Task<(NotificationItem, IEnumerable<string>)> SaveOrUpdate(NotificationItem notification)
        {
            //check for admin user


            var validationErrors = new List<string>();

            var user = userResolver.GetCurrentUserId();
            //from, to (opt), notificationType, message
            if(notification.FromDate == default(DateTime))
            {
                validationErrors.Add("From date must be set");
            }
            if (string.IsNullOrEmpty(notification.Message))
            {
                validationErrors.Add("Message must be set");
            }
            if (validationErrors.Any()) return (null, validationErrors);


            Db.Notification dbObject = null;
            if (notification.Id != Guid.Empty)
            {
                dbObject = await db.Notifications.FirstOrDefaultAsync(ps => ps.Id == notification.Id);
            }  

            if (dbObject != null) //update
            {                
                notification.Id = dbObject.Id;                
                mapper.Map(notification, dbObject);
            }
            else //add
            {               
                var newDbObject = mapper.Map<Db.Notification>(notification);
                if (newDbObject.Id == Guid.Empty)
                    newDbObject.Id = Guid.NewGuid();
                db.Notifications.Add(newDbObject);
                notification.Id = newDbObject.Id;
            }
            await db.SaveChangesAsync();

            return (await db.Notifications.ProjectTo<NotificationItem>(mapper.ConfigurationProvider)
                .FirstOrDefaultAsync(ps => ps.Id == notification.Id), null);
        }

        public async Task<(bool, IEnumerable<string>)> Delete(Guid id)
        {
            //check for admin user

            var validationErrors = new List<string>();
            var user = userResolver.GetCurrentUserId();
           
            if (id == Guid.Empty)
            {
                validationErrors.Add("Id must be set");
            }           

            Db.Notification dbObject = null;
            dbObject = await db.Notifications.FirstOrDefaultAsync(ps => ps.Id == id);            

            if (dbObject == null)
            {
                validationErrors.Add("Notification with id " + id + " does not exist");
            }
            else
            {
                db.Remove(dbObject);
                //db.Notifications.Remove() 
                await db.SaveChangesAsync();
            }
            return (true, validationErrors);
        }

    }
}
