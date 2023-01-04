using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using ChemDec.Api.Datamodel;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading.Tasks;

namespace ChemDec.Api.Infrastructure.Utils
{
    public class ConfigurationHelper
    {
        private readonly IConfiguration configuration;
        private readonly ChemContext dbContext;

        public ConfigurationHelper(IConfiguration config)
        {
            this.configuration = config;
        }

        public async Task<bool> UpdateConfigWithSecretsAsync() //Returns true if db-config is changed
        {
            bool res = false;
            var keyVaultUrl = configuration["KeyVaultEndpoint"];
            string clientId = configuration["azure:ClientId"];
            string clientSecret = configuration["ClientSecret"];

            TokenCredential credential;

            if (string.IsNullOrWhiteSpace(clientId) || string.IsNullOrWhiteSpace(clientSecret))
            {
                credential = new DefaultAzureCredential();
            }
            else
            {
                string tenantId = configuration["azure:TenantId"];
                credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            }

            var secretClient = new SecretClient(new Uri(keyVaultUrl), credential);
            var secrets = secretClient.GetPropertiesOfSecretsAsync();

            await foreach (SecretProperties secretProperties in secrets)
            {
                var url = secretProperties.Id.ToString();
                var secret = await secretClient.GetSecretAsync(url);

                if (secretProperties.Name == "ConnectionString" && secret.Value.Value != configuration[secretProperties.Name])
                {
                    res = true;
                }

                configuration[secretProperties.Name] = secret.Value.Value;
            }
           
            return res;
        }
    }
}
