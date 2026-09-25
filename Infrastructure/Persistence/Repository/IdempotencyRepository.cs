using Application.Contracts;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Persistence.Context;

namespace Persistence.Repository;

public class IdempotencyRepository(AppDbContext context) : IIdempotencyRepository
{
    public Task AcquireLockAsync(Guid userId, string key, CancellationToken cancellationToken)
    {
        var resource = $"wallet:idempotency:{userId:N}:{key}";
        return context.Database.ExecuteSqlInterpolatedAsync($@"
            DECLARE @result int;
            EXEC @result = sys.sp_getapplock
                @Resource = {resource},
                @LockMode = 'Exclusive',
                @LockOwner = 'Transaction',
                @LockTimeout = 10000;
            IF @result < 0
                THROW 51000, 'Idempotency kilidi alınamadı.', 1;", cancellationToken);
    }

    public Task<IdempotencyRecord?> GetAsync(Guid userId, string key, CancellationToken cancellationToken) =>
        context.IdempotencyRecords.SingleOrDefaultAsync(
            x => x.UserId == userId && x.Key == key && x.ExpiresAtUtc > DateTime.UtcNow,
            cancellationToken);

    public Task AddAsync(IdempotencyRecord record, CancellationToken cancellationToken) =>
        context.IdempotencyRecords.AddAsync(record, cancellationToken).AsTask();
}
