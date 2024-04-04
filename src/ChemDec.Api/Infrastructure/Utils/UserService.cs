using AutoMapper;
using AutoMapper.QueryableExtensions;
using ChemDec.Api.Datamodel;
using ChemDec.Api.Infrastructure.Security;
using ChemDec.Api.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChemDec.Api.Infrastructure.Utils
{
    public class UserService
    {
        private readonly UserResolver resolver;
        private readonly EquinorMsGraphHandler graphHandler;
        private readonly IMemoryCache cache;
        private readonly ChemContext db;
        private readonly IMapper mapper;
        private readonly IConfiguration config;

        public UserService(UserResolver resolver, EquinorMsGraphHandler graphHandler, IMemoryCache cache, ChemContext db, IMapper mapper, IConfiguration config)
        {
            this.resolver = resolver;
            this.graphHandler = graphHandler;
            this.cache = cache;
            this.db = db;
            this.mapper = mapper;
            this.config = config;
        }

        public Task<User> GetCurrentUser()
        {
            return GetUser(resolver.GetCurrentUserPrincipal());
        }

        public async Task<User> GetUser(ClaimsPrincipal userPrincipal)
        {
            if (string.IsNullOrEmpty(userPrincipal.Identity.Name))
                return null;

            var graphUser = await graphHandler.GetUserAsync(userPrincipal.Identity.Name);
            var roles = userPrincipal.Claims.Where(c => c.Type == ClaimTypes.Role);

            var released = string.Empty;
            DateTime date = DateTime.Now;
            if (config["released"]?.EndsWith("Z") == true) // assume date
            {

                if (DateTime.TryParse(config["released"], out date))
                {
                    released = date.ToString("dd MMM yyyy HH:mm");
                }
            }

            var res = new User
            {
                Upn = graphUser.UserPrincipalName,
                Name = graphUser.DisplayName,
                Email = graphUser.Mail,
                PortalEnv = config["env"],
                PortalBuild = config["build"],
                PortalRelease = string.IsNullOrEmpty(released) ? null : (DateTime?)date,
            };

            res.IsAffiliate = graphUser.UserType?.ToLower() == "guest";
            List<string> codes = new List<string>();
            codes = roles.Select(s => s.Value).ToList();

            var installations = await db.Installations.Where(w => codes.Contains(w.Code)).ProjectTo<InstallationReference>(mapper.ConfigurationProvider).ToListAsync();

            res.Roles = installations.Select(s => new Role { Id = s.Id.ToString(), Roletype = s.InstallationType == "plant" ? "Onshore" : "Offshore", Code = s.Code, Name = s.Name, Installation = s }).ToList();

            if (codes.Any(w => w == "CHEMICAL"))
            {
                res.Roles.Add(new Role { Id = "chemical", Roletype = "Chemical", Code = "Chemical responsible", Name = "Chemical responsible" });
            }

            if (codes.Any(w => w.ToLowerInvariant() == "admin"))
            {
                res.Roles.Add(new Role { Id = "admin", Roletype = "Admin", Code = "Admin", Name = "Administrator" });
            }
            if (res.Roles.Any() == false)
            {
                res.Roles = new List<Role>(){
                         new Role{Id = "noRole", Roletype="NoRoles", Code="NA", Name="No access", Installation=new InstallationReference{Code="NoAccess", Name="No Access" } }
                    };
            }
            return res;
        }

    }
}
