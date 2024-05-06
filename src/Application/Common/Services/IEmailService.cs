using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common.Services;

public interface IEmailService
{
    Task<EmailResponse> SendAsync(IEnumerable<string> emails, string subject, string body, CancellationToken cancellationToken = default);
}
