using Application.Common;
using Application.Common.Repositories;
using Domain.Installations;
using Domain.Users;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;

namespace ChemDec.Api.Infrastructure;

public sealed class UserProvider : IUserProvider
{
    private readonly IInstallationsRepository _installationRepository;
    public UserProvider(IInstallationsRepository installationRepository)
    {
        _installationRepository = installationRepository;
    }

    public async Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(claimsPrincipal.Identity.Name)) return null;

        IEnumerable<Claim> roles = claimsPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role);

        User user = new()
        {
            Upn = claimsPrincipal.GetDisplayName(),
            Name = claimsPrincipal.Claims.Where(c => c.Type == "name").FirstOrDefault().Value,
            Email = claimsPrincipal.GetDisplayName()
        };

        List<string> installationCodes = roles.Select(r => r.Value).ToList();
        List<Installation> installations = await _installationRepository.GetInstallationsByCodesAsync(installationCodes, cancellationToken);

        foreach (Installation installation in installations)
        {
            user.Roles.Add(new Role
            {
                Id = installation.Id,
                Roletype = installation.InstallationType == "plant" ? "OnShore" : "Offshore",
                Code = installation.Code,
                Name = installation.Name,
                Installation = installation
            });
        }

        if (installationCodes.Any(i => i.Equals("chemical", StringComparison.InvariantCultureIgnoreCase)))
        {
            user.Roles.Add(new Role
            {
                Id = Guid.Empty,
                Roletype = "Chemical",
                Code = "Chemical responsible",
                Name = "Chemical responsible",
            });
        }

        if (installationCodes.Any(i => i.Equals("admin", StringComparison.InvariantCultureIgnoreCase)))
        {
            user.Roles.Add(new Role
            {
                Id = Guid.Empty,
                Roletype = "Admin",
                Code = "Admin",
                Name = "Administrator"
            });
        }

        return user;
    }
}
