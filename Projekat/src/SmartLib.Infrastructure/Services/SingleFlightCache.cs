using System.Collections.Concurrent;
using Microsoft.Extensions.Caching.Distributed;

namespace SmartLib.Infrastructure.Services
{
    public class SingleFlightCache
    {
        private readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

        public async Task<T?> GetOrCreateAsync<T>(
            string key,
            TimeSpan ttl,
            Func<Task<T?>> factory,
            IDistributedCache cache)
        {
            // Try cache first
            var cached = await cache.GetRecordAsync<T>(key);
            if (cached != null) return cached;

            // Get or create lock for this key
            var lockObj = _locks.GetOrAdd(key, _ => new SemaphoreSlim(1, 1));

            try
            {
                await lockObj.WaitAsync();

                // Double-check after acquiring lock
                cached = await cache.GetRecordAsync<T>(key);
                if (cached != null) return cached;

                // Only one request hits the factory
                var value = await factory();
                if (value != null)
                {
                    await cache.SetRecordAsync(key, value, ttl);
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
