using System;
using System.Collections.Generic;

namespace ChemDec.Api.Model
{
    public class User
    {
        public string Name { get; set; }
        public string Email { get; set; }

        public List<Role> Roles { get; set; }
        public bool IsAffiliate { get; internal set; }
        public string Upn { get; internal set; }
        public string PortalEnv { get; internal set; }
        public string PortalBuild { get; internal set; }
        public DateTime? PortalRelease { get; internal set; }
    }
}
