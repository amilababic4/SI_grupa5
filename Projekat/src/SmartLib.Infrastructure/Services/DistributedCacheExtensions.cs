using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Caching.Distributed;

namespace SmartLib.Infrastructure.Services
{
    public static class DistributedCacheExtensions
    {
        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNameCaseInsensitive = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            ReferenceHandler = ReferenceHandler.IgnoreCycles
        };

        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string key)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrWhiteSpace(key)) return default;

            var data = await cache.GetAsync(key);
            if (data == null || data.Length == 0) return default;

            return JsonSerializer.Deserialize<T>(data, JsonOptions);
        }

        public static Task SetRecordAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan ttl)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrWhiteSpace(key)) return Task.CompletedTask;

            var data = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            return cache.SetAsync(key, data, options);
        }

        public static async Task<T?> GetOrCreateRecordAsync<T>(
            this IDistributedCache cache,
            string key,
            TimeSpan ttl,
            Func<Task<T?>> factory)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrWhiteSpace(key)) return default;
            if (factory == null) throw new ArgumentNullException(nameof(factory));

            var cached = await cache.GetRecordAsync<T>(key);
            if (cached != null)
            {
                return cached;
            }

            var value = await factory();
            if (value != null)
            {
                await cache.SetRecordAsync(key, value, ttl);
            }

            return value;
        }

        public static Task<byte[]?> GetBytesAsync(this IDistributedCache cache, string key)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrWhiteSpace(key)) return Task.FromResult<byte[]?>(null);

            return cache.GetAsync(key);
        }

        public static Task SetBytesAsync(this IDistributedCache cache, string key, byte[] value, TimeSpan ttl)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrWhiteSpace(key) || value == null || value.Length == 0) return Task.CompletedTask;

            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = ttl
            };

            return cache.SetAsync(key, value, options);
        }
    }
}
