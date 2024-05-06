using Domain.EmailNotifications;

namespace Application.Common.Repositories;

public interface IEmailNotificationsRepository
{
    Task AddAsync(EmailNotification email, CancellationToken cancellationToken = default);
    Task<List<EmailNotification>> GetUnProcessedEmailNotifications(int take = 5, CancellationToken cancellationToken = default);
    void Update(EmailNotification email);
}
