using ChemDec.Api.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ChemDec.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmailTestController : ControllerBase
    {
        private readonly MailSender _mailSender;
        public EmailTestController(MailSender mailSender)
        {
            _mailSender = mailSender;
        }

        [HttpPost]
        public async Task<IActionResult> TestEmail(string email)
        {
            var to = new List<string>
            {
                email
            };
            await _mailSender.SendMail(to, "Email test", "<h1>Test Mail</h1>");
            return Ok();
        }
    }
}
