using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System;

namespace ChemDec.Api
{
    public static class Utils
    {
        public static HashSet<string> GetCommaSeparatedConfigValue(IConfiguration config, string configKey)
        {
            var valueRaw = GetConfigValue(config, configKey);

            if (valueRaw.Contains(","))
            {
                var result = new HashSet<string>();

                foreach (var curValuePart in valueRaw.Split(',', StringSplitOptions.RemoveEmptyEntries))
                {
                    if (!string.IsNullOrWhiteSpace(curValuePart))
                    {
                        result.Add(curValuePart.Trim());
                    }
                }

                return result;
            }
            else
            {
                return new HashSet<string>() { valueRaw };
            }
        }

        public static string GetConfigValue(IConfiguration config, string configKey)
        {
            if (string.IsNullOrWhiteSpace(configKey))
            {
                throw new ArgumentNullException("configKey", "The parameter configKey was null or empty");
            }

            var configValue = config[configKey];

            if (string.IsNullOrWhiteSpace(configValue))
            {
                throw new Exception($"The configuration entry for {configKey} was empty");
            }

            return configValue;
        }
    }
}
