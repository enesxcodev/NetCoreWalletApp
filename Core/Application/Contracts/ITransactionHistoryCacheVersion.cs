namespace Application.Contracts;

public interface ITransactionHistoryCacheVersion
{
    Task<long> GetAsync(Guid walletId, CancellationToken cancellationToken);
    Task IncrementAsync(Guid walletId, CancellationToken cancellationToken);
}
