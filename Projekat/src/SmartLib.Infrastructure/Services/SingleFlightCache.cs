using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;

namespace SmartLib.Infrastructure.Services
{
    public class SingleFlightCache
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();
        private readonly IMemoryCache _memoryCache;

        public SingleFlightCache(IMemoryCache? memoryCache = null)
        {
            _memoryCache = memoryCache ?? new MemoryCache(new MemoryCacheOptions());
        }

        public async Task<T?> GetOrCreateAsync<T>(
            string key,
            TimeSpan ttl,
            Func<Task<T?>> factory,
            IDistributedCache cache)
        {
            return await GetOrCreateAsync(key, ttl, ttl, factory, cache);
        }

        public async Task<T?> GetOrCreateAsync<T>(
            string key,
            TimeSpan distributedTtl,
            TimeSpan memoryTtl,
            Func<Task<T?>> factory,
            IDistributedCache cache)
        {
            if (_memoryCache.TryGetValue(key, out T? memoryValue) && memoryValue != null)
            {
                return memoryValue;
            }

            // Try cache first
            var cached = await cache.GetRecordAsync<T>(key);
            if (cached != null) return cached;

            // Get or create lock for this key
            var lockObj = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            try
            {
                await lockObj.WaitAsync();

                // Double-check after acquiring lock
                if (_memoryCache.TryGetValue(key, out memoryValue) && memoryValue != null)
                {
                    return memoryValue;
                }

                cached = await cache.GetRecordAsync<T>(key);
                if (cached != null) return cached;

                // Only one request hits the factory
                var value = await factory();
                if (value != null)
                {
                    _memoryCache.Set(key, value, memoryTtl);
                    await cache.SetRecordAsync(key, value, distributedTtl);
                }

                return value;
            }
            finally
            {
                lockObj.Release();
                
                // Cleanup unused locks
                if (_locks.TryGetValue(key, out var @lock) && @lock.CurrentCount == 1)
                {
                    _locks.TryRemove(key, out _);
                }
            }
        }
    }
}
