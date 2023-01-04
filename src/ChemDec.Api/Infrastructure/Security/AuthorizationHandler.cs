using ChemDec.Api.Datamodel;
using System.Threading;

namespace ChemDec.Api.Infrastructure.Security
{
    public class AuthorizationHandler
    {

        const string RolePrefix = "";
        private readonly ChemContext db;
        private readonly MicrosoftGraphAuthenticationProvider gp;

        public AuthorizationHandler(ChemContext db, MicrosoftGraphAuthenticationProvider gp)
        {
            db = db;
            this.gp = gp;
        }

        static SemaphoreSlim syncLock = new SemaphoreSlim(1);

      /*  public async Task SyncChemicalRoles()
        {
            await syncLock.WaitAsync();
            try
            {
                var roles = await db.Roles.ToListAsync();
                var _handler = new EquinorMsGraphHandler(gp);
                var groups = await _handler.GetGroupsStartsWithAsync(RolePrefix);
                //OnPremisesSyncEnabled was the only propery that could seperate groups with same name, for instance SPTS
                var newRoles = groups.Where(x => x.OnPremisesSyncEnabled.HasValue && x.OnPremisesSyncEnabled.Value == true && !roles.Any(r => r.Code.Equals(x.MailNickname))).ToList();

                foreach (var newRole in newRoles)
                {
                    var desc = newRole.DisplayName;
                    if (newRole.DisplayName.StartsWith("APPL"))
                        desc = newRole.DisplayName.Substring(5);
                    desc = desc.Replace("-", "");
                    db.Roles.Add(new Role { AzureId = Guid.Parse(newRole.Id), Code = newRole.MailNickname, Description = desc });
                }
                db.SaveChanges();
            }
            finally
            {
                syncLock.Release();
            }

        }*/

    }
}
