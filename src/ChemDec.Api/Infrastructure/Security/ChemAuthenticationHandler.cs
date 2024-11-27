using ChemDec.Api.Datamodel;
using ChemDec.Api.Infrastructure.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

namespace ChemDec.Api.Infrastructure.Security
{
    public class ChemAuthenticationHandler : AuthorizationHandler<ChemAuthenticationRequirement>
    {
        private readonly UserService userService;
        private readonly IConfiguration config;
        private readonly ChemContext db;

        public ChemAuthenticationHandler(UserService userService, IConfiguration config, ChemContext db)
        {
            this.userService = userService;
            this.config = config;
            this.db = db;
        }
        protected override async Task HandleRequirementAsync(
        AuthorizationHandlerContext c, ChemAuthenticationRequirement requirement)
        {
            var user = await userService.GetCurrentUser();
            if (user == null)
            {
                //c.Fail();
                return;
            }

            if (requirement.MustBeTreatmentPlant)
            {
                if (user.Name != "plant") return; //just dummystuff, needs to be replaced with actual logic
            }

            c.Succeed(requirement);
        }
    }

    public class ChemAuthenticationRequirement : IAuthorizationRequirement
    {
        public bool MustBeTreatmentPlant { get; internal set; }
    }
}
