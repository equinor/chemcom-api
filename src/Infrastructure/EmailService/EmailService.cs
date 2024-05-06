using Application.Common.Services;
using Azure.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.EmailService;

public class EmailService : IEmailService
{
    private readonly IConfiguration _configuration;
    public EmailService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<EmailResponse> SendAsync(IEnumerable<string> emails, string subject, string body, CancellationToken cancellationToken = default)
    {
        try
        {
            string tenantId = _configuration["azure:TenantId"];
            string clientId = _configuration["azure:ClientId"];
            string clientSecret = _configuration["azure:ClientSecret"];

            ClientSecretCredential clientSecretCredential = new(tenantId, clientId, clientSecret);
            ChainedTokenCredential chainedTokenCredential = new(new WorkloadIdentityCredential(), clientSecretCredential);
            GraphServiceClient graphClient = new(chainedTokenCredential);

            List<Recipient> recipients = new List<Recipient>();
            foreach (var emailAddress in emails)
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
                    Content = body
                },
                ToRecipients = recipients
            };

            bool saveToSentItems = true;
            await graphClient.Users[_configuration["FromEmailAddress"]]
                             .SendMail(message, saveToSentItems)
                             .Request()
                             .PostAsync(cancellationToken);
            return EmailResponse.Success();
        }
        catch (Exception ex)
        {
            //TODO: log exception
            return EmailResponse.Fail(ex.Message);
        }        
    }
}
