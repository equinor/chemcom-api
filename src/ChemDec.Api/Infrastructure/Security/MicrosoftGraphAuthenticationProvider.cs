using ChemDec.Api.Datamodel;
using Microsoft.Extensions.Configuration;
using Microsoft.Graph;
using System.Net.Http;
using System.Threading.Tasks;

namespace ChemDec.Api.Infrastructure.Security
{

    public class MicrosoftGraphAuthenticationProvider : IAuthenticationProvider
    {
        private readonly IConfiguration config;
        private readonly ChemContext db;

        public MicrosoftGraphAuthenticationProvider(IConfiguration config, ChemContext db)
        {
            this.config = config;
            this.db = db;
        }
        public async Task AuthenticateRequestAsync(HttpRequestMessage request)
        {
            //string clientId = config["azure:ClientId"];
            //string clientSecret = config["ClientSecret"];
            //string authority = $"{config["azure:Instance"]}{config["azure:TenantName"]}";
            //string resource = $"{config["azure:GraphApiUrl"]}";
            //var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(authority);

            //ClientCredential creds = new ClientCredential(clientId, clientSecret);

            //AuthenticationResult authResult = await authContext.AcquireTokenAsync(resource, creds);

            //request.Headers.Add("Authorization", "Bearer " + authResult.AccessToken);
        }


        //public async Task<string> GetGraphAccessForUserTokenAsync(string signInUserId, string userObjectId)
        //{
        //    var Authority = config["azure:AADInstance"] + config["azure:TenantId"];
        //    var creddd = new ClientSecretCredential(config["azure:TenantId"], config["azure:ClientId"], config["azure:ClientSecret"]);
        //    var clientCredential = new ClientCredential(config["azure:ClientId"], config["azure:ClientSecret"]);
        //    var userIdentifier = new UserIdentifier(userObjectId, UserIdentifierType.UniqueId);

        //    // create auth context
        //    var authContext = new Microsoft.IdentityModel.Clients.ActiveDirectory.AuthenticationContext(Authority, new AdalTokenCache(db, signInUserId));

        //    AuthenticationResult result = await authContext.AcquireTokenSilentAsync(config["azure:GraphApiUrl"], clientCredential, userIdentifier);
        //    return result.AccessToken;
        //}
    }
}
