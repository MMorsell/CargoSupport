using LazyCache;
using Microsoft.Extensions.Caching.Memory;
using System;

namespace CargoSupport.Helpers
{
    /// <summary>
    /// Application Cache helper class to save {T} to the memory
    /// </summary>
    public static class CacheHelper
    {
        /// <summary>
        /// Retrieves the cache from the cache, then resaves it
        /// </summary>
        /// <typeparam name="T">Type of the object(s)</typeparam>
        /// <param name="cache"><see cref="IAppCache"/> object</param>
        /// <param name="cacheKey">The key to save the cache as</param>
        /// <param name="absoluteLifetimeOfCache">The absolute life span of the new cache that is saved</param>
        public static void ReAddCache<T>(IAppCache cache, string cacheKey, TimeSpan absoluteLifetimeOfCache)
        {
            var res = cache.Get<T>(cacheKey);
            var policy = new MemoryCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteLifetimeOfCache
            };
            cache.Add(cacheKey, res, policy);
        }

        /// <summary>
        /// Removes the cache if it exists, then saves the <paramref name="contentToCache"/>
        /// </summary>
        /// <typeparam name="T">Type of the object(s)</typeparam>
        /// <param name="cache"><see cref="IAppCache"/> object</param>
        /// <param name="cacheKey">The key to save the cache as</param>
        /// <param name="absoluteLifetimeOfCache">The absolute life span of the new cache that is saved</param>
        /// <param name="contentToCache">The object(s) to cache</param>
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