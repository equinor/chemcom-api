using Application.Common.Repositories;
using Application.Common.Services;
using Domain.EmailNotifications;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace ChemDec.Api.BackgroundJobs;

[DisallowConcurrentExecution]
public class EmailNotificationJob : IJob
{
    //TODO: Add exception handling
    private readonly IEmailNotificationsRepository _emailNotificationsRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmailService _emailService;
    private readonly ILogger<EmailNotificationJob> _logger;
    public EmailNotificationJob(IEmailNotificationsRepository emailNotificationsRepository, IUnitOfWork unitOfWork, IEmailService emailService, ILogger<EmailNotificationJob> logger)
    {
        _emailNotificationsRepository = emailNotificationsRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            List<EmailNotification> emailNotifications = await _emailNotificationsRepository.GetUnProcessedEmailNotifications(take: 5, context.CancellationToken);

            if (emailNotifications.Any())
            {
                _logger.LogInformation($"Processing {emailNotifications.Count} email notifications");
                foreach (EmailNotification emailNotification in emailNotifications)
                {
                    IEnumerable<string> emails = emailNotification.Recipients.Split(",");
                    EmailResponse emailResponse = await _emailService.SendAsync(emails, emailNotification.Subject, emailNotification.Body, context.CancellationToken);
                    if (!emailResponse.IsSuccessful)
                    {
                        emailNotification.IsSent = false;
                        emailNotification.ErrorMessage = emailResponse.Error;
                        _emailNotificationsRepository.Update(emailNotification);
                        await _unitOfWork.CommitChangesAsync(context.CancellationToken);
                        _logger.LogError("Error sending email notification {emailNotificationId}", emailNotification.Id);
                        continue;
                    }

                    emailNotification.IsSent = true;
                    emailNotification.SentAt = DateTime.UtcNow;
                    _emailNotificationsRepository.Update(emailNotification);
                    await _unitOfWork.CommitChangesAsync(context.CancellationToken);
                    _logger.LogInformation("Email notification {emailNotificationId} has been sent", emailNotification.Id);
                }
            }
            else
            {
                _logger.LogInformation("No email notifications to process");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error processing email notifications");
            throw;
        }
    }
}
