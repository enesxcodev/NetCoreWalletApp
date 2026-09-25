using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Persistence.Context;

namespace Persistence.Services;

public class IdempotencyCleanupService(
    IServiceScopeFactory scopeFactory,
    ILogger<IdempotencyCleanupService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        using var timer = new PeriodicTimer(TimeSpan.FromHours(1));
        do
        {
            try
            {
                await using var scope = scopeFactory.CreateAsyncScope();
                var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();
                var deleted = await context.IdempotencyRecords
                    .Where(x => x.ExpiresAtUtc <= DateTime.UtcNow)
                    .ExecuteDeleteAsync(stoppingToken);
                if (deleted > 0)
                    logger.LogInformation("{Count} süresi dolmuş idempotency kaydı silindi.", deleted);
            }
            catch (Exception exception) when (!stoppingToken.IsCancellationRequested)
            {
                logger.LogError(exception, "Idempotency kayıtları temizlenemedi.");
            }
        }
        while (await timer.WaitForNextTickAsync(stoppingToken));
    }
}
