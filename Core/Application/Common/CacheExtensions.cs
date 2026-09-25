using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
namespace Application.Common
{
    public static class CacheExtensions
    {
        public static async Task SetAsync<T>(this IDistributedCache cache, string key, T value, TimeSpan absoluteExpiration, CancellationToken cancellationToken = default)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = absoluteExpiration // Cache ömrü (Örn: 5 dk)
            };

            var json = JsonSerializer.Serialize(value);
            await cache.SetStringAsync(key, json, options, cancellationToken);
        }

        // 🎯 Redis'ten veriyi okuyup otomatik kendi DTO tipimize çeviren metot
        public static async Task<T?> GetAsync<T>(this IDistributedCache cache, string key, CancellationToken cancellationToken = default)
        {
            var json = await cache.GetStringAsync(key, cancellationToken);
            if (json is null) return default;

            return JsonSerializer.Deserialize<T>(json);
        }

    }
}
