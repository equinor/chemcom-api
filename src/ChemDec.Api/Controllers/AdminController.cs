using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using ChemDec.Api.Controllers.Handlers;
using ChemDec.Api.Model;
using Microsoft.EntityFrameworkCore;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ChemDec.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdminController : ControllerBase
    {
        private readonly AdminHandler handler;

        public AdminController(AdminHandler handler)
        {
            this.handler = handler;
        }

        [HttpGet]
        [Route("currentnotification")]
        public NotificationItem GetCurrentNotification()
        {
            var notification = handler.GetCurrentNotification();

            if (notification == null)
            {
                return null;
            }

            return notification; //new NotificationItem(notification);
        }

        [HttpGet]
        [Route("notifications")]
        public async Task<ActionResult<List<NotificationItem>>> GetNotifications() 
        {
            var notifications = handler.GetNotifications();
            return await notifications.OrderBy(o => o.FromDate).ToListAsync();            
        }

        [HttpPost]
        [Route("notifications")]
        public async Task<ActionResult<NotificationItem>> SaveNotification([FromBody] NotificationItem notification)
        {
            (var savedNotification, var validationErrors) = await handler.SaveOrUpdate(notification);

            if (validationErrors != null && validationErrors.Any())
            {
                return BadRequest(new { error = validationErrors });
            }

            return savedNotification;
        }

        [HttpDelete]
        [Route("notifications/{id}")]
        public async Task<ActionResult<dynamic>> DeleteNotification(Guid id)
        {
            (var ok, var validationErrors) = await handler.Delete(id);
            
            if(validationErrors != null && validationErrors.Any())
            {
                return BadRequest(new { error = validationErrors });
            }
            return new { Res = "Deleted" };
        }
    }
}
