namespace SmartLib.Infrastructure.Services
{
    public static class RedisConfigHelper
    {
        /// <summary>
        /// Builds Redis connection string from environment variables.
        /// Returns null when no Redis env vars are set (triggers in-memory fallback).
        /// If UPSTASH_REDIS_CONNECTION_STRING is set, returns it as-is (user-provided full string).
        /// If UPSTASH_REDIS_URL is set, builds connection string with timeout/retry params.
        /// </summary>
        public static string? BuildRedisConfiguration()
        {
            var connString = Environment.GetEnvironmentVariable("UPSTASH_REDIS_CONNECTION_STRING");
            if (!string.IsNullOrWhiteSpace(connString))
                return connString; // User-provided full connection string — use as-is

            var redisUrl = Environment.GetEnvironmentVariable("UPSTASH_REDIS_URL");
            if (string.IsNullOrWhiteSpace(redisUrl))
                return null;

            if (!Uri.TryCreate(redisUrl, UriKind.Absolute, out var redisUri))
                return null;

            var password = Environment.GetEnvironmentVariable("UPSTASH_REDIS_PASSWORD");
            if (string.IsNullOrWhiteSpace(password) && !string.IsNullOrWhiteSpace(redisUri.UserInfo))
            {
                var parts = redisUri.UserInfo.Split(':', 2);
                if (parts.Length == 2)
                    password = parts[1];
            }

            if (string.IsNullOrWhiteSpace(password))
                return null;

            var port = redisUri.Port > 0 ? redisUri.Port : 6379;
            // Only add timeout/retry params when we build the string ourselves
            return $"{redisUri.Host}:{port},password={password},ssl=True,abortConnect=False,connectTimeout=5000,syncTimeout=5000,connectRetry=2";
        }
    }
}
