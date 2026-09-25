using Application.Contracts;
using StackExchange.Redis;

namespace Persistence.Services;

public class TransactionHistoryCacheVersion(IConnectionMultiplexer redis) : ITransactionHistoryCacheVersion
{
    private static string Key(Guid walletId) => $"tx_history_version:{walletId:N}";

    public async Task<long> GetAsync(Guid walletId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        var value = await redis.GetDatabase().StringGetAsync(Key(walletId));
        return value.TryParse(out long version) ? version : 0;
    }

    public async Task IncrementAsync(Guid walletId, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();
        await redis.GetDatabase().StringIncrementAsync(Key(walletId));
    }
}
