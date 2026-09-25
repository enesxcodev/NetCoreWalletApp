using Application.Contracts;
using Application.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Application.Consumers;

public class MoneyTransferredConsumer(
    ITransactionAuditService auditService,
    ITransactionHistoryCacheVersion cacheVersion,
    ILogger<MoneyTransferredConsumer> logger
) : IConsumer<MoneyTransferredEvent> // MassTransit interface'i
{
    public async Task Consume(ConsumeContext<MoneyTransferredEvent> context)
    {
        var message = context.Message;

        logger.LogInformation(
            "Transfer event'i alındı. MessageId: {MessageId}, CorrelationId: {CorrelationId}, TransactionId: {TransactionId}, IdempotencyKey: {IdempotencyKey}",
            context.MessageId, context.CorrelationId, message.TransactionId, message.IdempotencyKey);

        // 🚀 Az önce Handler'da doğrudan çağırdığımız Mongo kayıt kodunu artık arka planda tetikliyoruz
        await auditService.IdempotentLogAsync(
            message.TransactionId,
            message.SenderWalletId,
            message.SenderWalletCode,
            message.ReceiverWalletId,
            message.ReceiverWalletCode,
            message.Amount,
            message.Description,
            "Out",
            context.CancellationToken
        );

        await cacheVersion.IncrementAsync(message.SenderWalletId, context.CancellationToken);
        await cacheVersion.IncrementAsync(message.ReceiverWalletId, context.CancellationToken);

        logger.LogInformation("MongoDB audit kaydı ve Redis cache invalidation tamamlandı. TransactionId: {TransactionId}", message.TransactionId);
    }
}
