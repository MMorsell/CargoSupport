using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace CargoSupport.Helpers
{
    public static class CacheHelper
    {
        public static void ReaddCache<T>(IAppCache cache, string cacheKey, TimeSpan absoluteLifetimeOfCache)
        {
            var res = cache.Get<T>(cacheKey);
            var policy = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteLifetimeOfCache
            };
            cache.Add(cacheKey, res, policy);
        }

        public static void UpdateCache<T>(IAppCache cache, string cacheKey, TimeSpan absoluteLifetimeOfCache, T contentToCache)
        {
            cache.Remove(cacheKey);
            var policy = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteLifetimeOfCache
            };
            cache.Add(cacheKey, contentToCache, policy);
        }
    }
}