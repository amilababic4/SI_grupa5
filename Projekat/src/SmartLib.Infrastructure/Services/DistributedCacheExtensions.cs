using System.Collections.Concurrent;
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

        private static readonly Action<Exception> LogRedisError = ex =>
            Console.WriteLine($"[Redis] Cache operation failed, falling back: {ex.Message}");

        // Circuit breaker: stop hammering Redis after N consecutive failures
        private static int _consecutiveFailures = 0;
        private static DateTime _circuitOpenUntil = DateTime.MinValue;
        private const int FailureThreshold = 5;
        private static readonly TimeSpan Cooldown = TimeSpan.FromSeconds(30);

        private static bool IsCircuitOpen()
        {
            if (_consecutiveFailures < FailureThreshold) return false;
            if (DateTime.UtcNow >= _circuitOpenUntil)
            {
                _consecutiveFailures = 0;
                Console.WriteLine("[Redis] Circuit breaker half-open, allowing retry");
                return false;
            }
            return true;
        }

        private static void RecordSuccess()
        {
            _consecutiveFailures = 0;
        }

        private static void RecordFailure()
        {
            var count = Interlocked.Increment(ref _consecutiveFailures);
            if (count >= FailureThreshold)
            {
                _circuitOpenUntil = DateTime.UtcNow.Add(Cooldown);
                Console.WriteLine($"[Redis] Circuit breaker OPEN after {count} consecutive failures, cooling down for {Cooldown.TotalSeconds}s");
            }
        }

        public static async Task<T?> GetRecordAsync<T>(this IDistributedCache cache, string key)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrWhiteSpace(key)) return default;

            if (IsCircuitOpen()) return default;

            try
            {
                var data = await cache.GetAsync(key);
                if (data == null || data.Length == 0) return default;
                RecordSuccess();
                return JsonSerializer.Deserialize<T>(data, JsonOptions);
            }
            catch (Exception ex)
            {
                LogRedisError(ex);
                RecordFailure();
                return default;
            }
        }

        public static async Task SetRecordAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan ttl)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrWhiteSpace(key)) return;

            if (IsCircuitOpen()) return;

            try
            {
                var data = JsonSerializer.SerializeToUtf8Bytes(value, JsonOptions);
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl
                };
                await cache.SetAsync(key, data, options);
                RecordSuccess();
            }
            catch (Exception ex)
            {
                LogRedisError(ex);
                RecordFailure();
            }
        }

        public static async Task<byte[]?> GetBytesAsync(this IDistributedCache cache, string key)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrWhiteSpace(key)) return null;

            if (IsCircuitOpen()) return null;

            try
            {
                var result = await cache.GetAsync(key);
                RecordSuccess();
                return result;
            }
            catch (Exception ex)
            {
                LogRedisError(ex);
                RecordFailure();
                return null;
            }
        }

        public static async Task SetBytesAsync(this IDistributedCache cache, string key, byte[] value, TimeSpan ttl)
        {
            if (cache == null) throw new ArgumentNullException(nameof(cache));
            if (string.IsNullOrWhiteSpace(key) || value == null || value.Length == 0) return;

            if (IsCircuitOpen()) return;

            try
            {
                var options = new DistributedCacheEntryOptions
                {
                    AbsoluteExpirationRelativeToNow = ttl
                };
                await cache.SetAsync(key, value, options);
                RecordSuccess();
            }
            catch (Exception ex)
            {
                LogRedisError(ex);
                RecordFailure();
            }
        }

        public static async Task<T?> GetOrCreateRecordAsync<T>(
            this IDistributedCache cache,
            string key,
            TimeSpan ttl,
            Func<Task<T?>> factory)
        {
            if (IsCircuitOpen())
            {
                return await factory();
            }

            try
            {
                var cached = await cache.GetRecordAsync<T>(key);
                if (cached != null) return cached;
            }
            catch (Exception ex)
            {
                LogRedisError(ex);
            }

            var data = await factory();
            if (data != null)
            {
                try
                {
                    await cache.SetRecordAsync(key, data, ttl);
                }
                catch (Exception ex)
                {
                    LogRedisError(ex);
                }
            }

            return data;
        }
    }
}
