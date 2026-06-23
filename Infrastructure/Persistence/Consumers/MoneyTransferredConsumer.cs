using Application.Contracts;
using Application.Contracts.Events;
using MassTransit;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace Application.Consumers;

public class MoneyTransferredConsumer(
    ITransactionAuditService auditService, // 🎯 Eski dostu buraya enjekte ediyoruz!
    ILogger<MoneyTransferredConsumer> logger
) : IConsumer<MoneyTransferredEvent> // MassTransit interface'i
{
    public async Task Consume(ConsumeContext<MoneyTransferredEvent> context)
    {
        var message = context.Message;

        logger.LogInformation("Kuyruktan mesaj yakalandı! TransactionId: {Id}", message.TransactionId);

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

        logger.LogInformation("MongoDB asenkron audit kaydı başarıyla tamamlandı!");
    }
}