using Microsoft.Extensions.Caching.Distributed;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using StackExchange.Redis;
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

        public static async Task RemoveByPrefixAsync(this IDistributedCache cache, string prefix)
        {
            // IDistributedCache üzerinden gerçek Redis bağlantısına ulaşıyoruz
            var connectionMultiplexer = ConnectionMultiplexer.Connect("localhost:6379");
            var server = connectionMultiplexer.GetServer(connectionMultiplexer.GetEndPoints().First());

            // InstanceName önekini de hesaba katarak ilgili tüm keyleri buluyoruz (Örn: WalletApp_tx_history:walletId:*)
            var keys = server.Keys(pattern: $"*{prefix}*").ToArray();

            foreach (var key in keys)
            {
                await cache.RemoveAsync(key.ToString().Replace("WalletApp_", "")); // prefixi temizleyip sildir
            }
        }
    }
}
