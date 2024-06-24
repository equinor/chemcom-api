using Domain.Users;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Application.Common;

public interface IUserProvider
{
    Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default);
}
