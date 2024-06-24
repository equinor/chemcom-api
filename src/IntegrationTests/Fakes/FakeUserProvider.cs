using Application.Common;
using Application.Common.Repositories;
using Domain.Installations;
using Domain.Users;
using IntegrationTests.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace IntegrationTests.Fakes;

public class FakeUserProvider : IUserProvider
{
    private readonly IInstallationsRepository _installationRepository;
    public FakeUserProvider(IInstallationsRepository installationRepository)
    {
        _installationRepository = installationRepository;
    }


    public async Task<User> GetUserAsync(ClaimsPrincipal claimsPrincipal, CancellationToken cancellationToken = default)
    {        
        User user = new()
        {
            Name = "abcd efgh",
            Upn = "abcd@equinor.com",
            Email = "abcd@equinor.com"
        };

        //TODO: Mock the claims principal. Add the values to the constants. 
        List<string> installationCodes = ["OsebergC", "Sture", "admin", "chemical"];
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
