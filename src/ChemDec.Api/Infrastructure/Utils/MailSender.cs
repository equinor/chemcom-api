using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;

namespace ChemDec.Api.Infrastructure.Utils
{
    public class MailSender
    {
        private readonly IConfiguration _config;

        public MailSender(IConfiguration config)
        {
            _config = config;
        }      

        public async Task SendMail(IEnumerable<string> to, string subject, string infoHtml)
        {
            try
            {
                string tenantId = _config["azure:TenantId"];
                string clientId = _config["azure:ClientId"];
                string clientSecret = _config["azure:ClientSecret"];               

                ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret);
                ChainedTokenCredential chainedTokenCredential = new(new WorkloadIdentityCredential(), clientSecretCredential);
                GraphServiceClient graphClient = new(chainedTokenCredential);

                List<Recipient> recipients = new List<Recipient>();
                foreach (var emailAddress in to)
                {
                    Recipient recipient = new Recipient
                    {
                        EmailAddress = new EmailAddress { Address = emailAddress }
                    };
                    recipients.Add(recipient);
                }

                Message message = new()
                {
                    Subject = subject,
                    Body = new ItemBody
                    {
                        ContentType = BodyType.Html,
                        Content = infoHtml
                    },
                    ToRecipients = recipients
                };
                bool saveToSentItems = true;
                await graphClient.Users[_config["FromEmailAddress"]]
                                 .SendMail(message, saveToSentItems)
                                 .Request()
                                 .PostAsync();               
            }
            catch (Exception ex)
            {
                throw new Exception("Email notification failed:", ex);
            }
        }

    }
}
