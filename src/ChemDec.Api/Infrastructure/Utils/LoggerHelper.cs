using ChemDec.Api.Model;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using static ChemDec.Api.Controllers.Handlers.ShipmentHandler;

namespace ChemDec.Api.Infrastructure.Utils
{
    public class LoggerHelper
    {
        private readonly ILogger<LoggerHelper> logger;

        public LoggerHelper(ILogger<LoggerHelper> logger)
        {
            this.logger = logger;
        }

 
        public void LogEvent<T>(User user, PlantReference from, PlantReference to, Operation? operation, DetailedOperation? details, string eventName, T payload)
        {
            try
            {
                var jPayload = (JObject)JToken.FromObject(payload);
                var flattened = Flatten(jPayload, user, from, to, operation, details);

                var flattenedJson = JsonConvert.SerializeObject(flattened);

                logger.LogInformation("Event: {EventName} | Flattened Data: {FlattenedData}", eventName, flattenedJson);
            }
            catch (Exception ex)
            {
                logger.LogError(new EventId(1), ex, "Tracking failed for event {EventName}", eventName);
            }
        }

        public Dictionary<string, string> Flatten(JObject jsonObject, User user, PlantReference from, PlantReference to, Operation? operation, DetailedOperation? details)
        {
            var prefix = user == null ? null : user + ".";
            try
            {
                IEnumerable<JToken> jTokens = jsonObject.Descendants().Where(p => p.Count() == 0);
                Dictionary<string, string> results = jTokens.Aggregate(new Dictionary<string, string>(),
                    (properties, jToken) =>
                    {
                        properties.Add($"{jToken.Path}", jToken.ToString());
                        return properties;
                    });
                if (user != null) results.Add("userUpn", user.Upn);
                if (user != null) results.Add("userName", user.Name);
                if (user != null) results.Add("userEmail", user.Email);
                if (from != null) results.Add("from", from.Code);
                if (to != null) results.Add("to", to.Code);
                if (operation != null) results.Add("operation", operation.ToString());
                if (details != null) results.Add("detailedOperation", details.ToString());
                return results;
            }
            catch (Exception ex)
            {
                return new Dictionary<string, string>() { { "value", ex.ToString() } };
            }


        }

    }
}
