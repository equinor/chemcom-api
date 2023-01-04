using LazyCache;
using System.Collections.Generic;

namespace ChemDec.Api.Infrastructure.Utils
{
    public enum CacheCategories
    {
        Filter,
        Projects,
        Roles,
        User
    }
    public class CacheHelper
    {
        private readonly IAppCache appCache;

        public CacheHelper(IAppCache appCache)
        {
            this.appCache = appCache;
        }

        public void ClearCache(string startsWith)
        {
           /* TODO: Fix for .NET COre
            * var cacheKeys = appCache.Where(kvp => kvp.Key.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase))
                   .Select(kvp => kvp.Key);
            foreach (var cacheKey in cacheKeys)
            {
                appCache.ObjectCache.Remove(cacheKey);
            }
            */

        }
        public List<object> GetAll(string startsWith)
        {
            var res = new List<object>();
           /* var cacheKeys = appCache.ObjectCache.Where(kvp => kvp.Key.StartsWith(startsWith, StringComparison.InvariantCultureIgnoreCase))
                   .Select(kvp => kvp.Key);
            foreach (var cacheKey in cacheKeys)
            {
                res.Add(appCache.ObjectCache[cacheKey]);
            }
            */
            return res;

        }
    }
    
}
