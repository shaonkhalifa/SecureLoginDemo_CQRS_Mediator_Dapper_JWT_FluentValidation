using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace MadiatrProject.Cache
{
    public class CacheService : ICacheService
    {
        private static readonly ConcurrentDictionary<string, bool> Cachekes = new ConcurrentDictionary<string, bool>();
        private readonly IDistributedCache _distributedCache;

        public CacheService(IDistributedCache distributedCache)
        {
            _distributedCache = distributedCache;
        }

        public async Task<T?> GetAsync<T>(string key, CancellationToken cancellationToken = default) where T : class
        {
            string? cacheValue = await _distributedCache.GetStringAsync(key, cancellationToken);
            if (cacheValue == null)
            {
                return null;
            }
            T? value = JsonConvert.DeserializeObject<T>(cacheValue);
            return value;
        }

        public async Task<T> GetAsync<T>(string key, Func<Task<T>> fatory, CancellationToken cancellationToken = default) where T : class
        {
            T? cashValue= await GetAsync<T>(key, cancellationToken);
            if (cashValue is not null)
            {
                return cashValue;
            }
            cashValue = await fatory();
            await SetAsync(key, cashValue,cancellationToken);

            return cashValue;
            
        }

        public async Task RemoveAsync(string key, CancellationToken cancellationToken = default)
        {
            await _distributedCache.RemoveAsync(key, cancellationToken);
            Cachekes.TryRemove(key, out bool _);
        }

        public async Task RemoveByPrefixAsync(string prefixkey, CancellationToken cancellationToken = default)
        {

            //foreach (var key in Cachekes.Keys) 
            //{
            //    if (key.StartsWith(prefixkey))
            //    {
            //        await RemoveAsync(key, cancellationToken);  
            //    }
            //}

            IEnumerable<Task> tasks = Cachekes
                 .Keys
                 .Where(k => k.StartsWith(prefixkey))
                 .Select(a => RemoveAsync(a, cancellationToken));
            await Task.WhenAll(tasks);

        }

        public async Task SetAsync<T>(string key, T value, CancellationToken cancellationToken = default) where T : class
        {
            string cacheValue = JsonConvert.SerializeObject(value);
            await _distributedCache.SetStringAsync(key, cacheValue, cancellationToken);
            Cachekes.TryAdd(cacheValue, false);

        }
    }
}
