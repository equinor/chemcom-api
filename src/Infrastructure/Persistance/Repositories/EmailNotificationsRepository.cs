﻿using Application.Common.Repositories;
using Domain.EmailNotifications;
using Microsoft.EntityFrameworkCore;
using System;
namespace Infrastructure.Persistance.Repositories;

public class EmailNotificationsRepository : IEmailNotificationsRepository
{
    private readonly ApplicationDbContext _dbContext;
    public EmailNotificationsRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(EmailNotification email, CancellationToken cancellationToken = default)
    {
        await _dbContext.EmailNotifications.AddAsync(email, cancellationToken);
    }

    public void Update(EmailNotification email)
    {
        _dbContext.EmailNotifications.Update(email);
    }

    public async Task<List<EmailNotification>> GetUnProcessedEmailNotifications(int take = 5, CancellationToken cancellationToken = default)
    {
        return await _dbContext.EmailNotifications.
                      Where(x => !x.IsSent).
                      Take(take).
                      Select(x => new EmailNotification
                      {
                          Id = x.Id,
                          Subject = x.Subject,
                          Body = x.Body,
                          Recipients = x.Recipients,
                          SentAt = x.SentAt,
                          IsSent = x.IsSent,
                          EmailNotificationType = Convert.ToInt32(x.EmailNotificationType),
                          ErrorMessage = x.ErrorMessage,
                      }).
                      ToListAsync(cancellationToken);
    }
}
