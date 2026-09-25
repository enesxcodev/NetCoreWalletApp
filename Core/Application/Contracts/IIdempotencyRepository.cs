using Domain.Entities;

namespace Application.Contracts;

public interface IIdempotencyRepository
{
    Task AcquireLockAsync(Guid userId, string key, CancellationToken cancellationToken);
    Task<IdempotencyRecord?> GetAsync(Guid userId, string key, CancellationToken cancellationToken);
    Task AddAsync(IdempotencyRecord record, CancellationToken cancellationToken);
}
