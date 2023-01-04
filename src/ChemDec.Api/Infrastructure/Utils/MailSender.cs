using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ChemDec.Api.Infrastructure.Utils
{
    public class MailSender
    {
        private readonly IConfiguration config;

        public MailSender(IConfiguration config)
        {
            this.config = config;
        }

        public async Task SendMail(IEnumerable<string> to, string subject, string infoHtml, string info)
        {
            try
            {
                SmtpClient client = new SmtpClient("mrrr.statoil.com");
                client.UseDefaultCredentials = false;
                client.Port = 25;
                client.EnableSsl = true;
                client.Credentials = new NetworkCredential("chemcom@equinor.com", config["smtpPw"]);

                MailMessage mailMessage = new MailMessage();
                mailMessage.From = new MailAddress("chemcom@equinor.com");
                foreach (var a in to)
                {
                    mailMessage.To.Add(a);
                }
                mailMessage.Body = infoHtml;
                mailMessage.IsBodyHtml = true;
                mailMessage.Subject = subject;
                await client.SendMailAsync(mailMessage);
            }
            catch (Exception ex)
            {
                throw new Exception("Email notification failed:", ex);
            }
        }
     
    }
}
