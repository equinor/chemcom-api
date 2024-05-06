using Application.Common.Repositories;
using Application.Common.Services;
using Domain.EmailNotifications;
using Microsoft.Extensions.Hosting;
using Quartz;
using System;
using System.Collections.Generic;
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
    public EmailNotificationJob(IEmailNotificationsRepository emailNotificationsRepository, IUnitOfWork unitOfWork, IEmailService emailService)
    {
        _emailNotificationsRepository = emailNotificationsRepository;
        _unitOfWork = unitOfWork;
        _emailService = emailService;
    }

    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            List<EmailNotification> emailNotifications = await _emailNotificationsRepository.GetUnProcessedEmailNotifications(take: 5, context.CancellationToken);
            foreach (EmailNotification emailNotification in emailNotifications)
            {
                IEnumerable<string> emails = emailNotification.Recipients.Split(",");
                EmailResponse emailResponse = await _emailService.SendAsync(emails, emailNotification.Subject, emailNotification.Body);
                if (!emailResponse.IsSuccessful)
                {
                    emailNotification.ErrorMessage = emailResponse.Error;
                    _emailNotificationsRepository.Update(emailNotification);
                    await _unitOfWork.CommitChangesAsync(context.CancellationToken);
                    continue;
                }

                emailNotification.IsSent = true;
                emailNotification.SentAt = DateTime.UtcNow;
                _emailNotificationsRepository.Update(emailNotification);
                await _unitOfWork.CommitChangesAsync(context.CancellationToken);
            }
        }
        catch (Exception ex)
        {
            //Add logging 
            throw;
        }
    }
}
