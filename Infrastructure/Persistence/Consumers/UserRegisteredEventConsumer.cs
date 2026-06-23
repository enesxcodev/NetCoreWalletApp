using Application.Common.Events;
using Domain.Entities;
using MassTransit;
using Persistence.Context;

namespace Persistence.Consumers;
public class UserRegisteredEventConsumer(AppDbContext context) : IConsumer<UserRegisteredEvent>
{
    public async Task Consume(ConsumeContext<UserRegisteredEvent> contextMessage)
    {
        // 1. Kargodan gelen UserId bilgisini açıyoruz
        Guid registeredUserId = contextMessage.Message.UserId;

        // 2. Senin o yazdığın akıllı Entity constructor'ını tetikliyoruz
        // İçeride cüzdan kodunu (WLT-XXXX) otomatik kendisi üretiyor!
        var newWallet = new Wallet(registeredUserId);

        // 3. Sessizce veri tabanına cüzdanı mühürlüyoruz
        await context.Wallets.AddAsync(newWallet);
        await context.SaveChangesAsync();

        
        Console.WriteLine($"[WALLET WORKER] -> Kullanıcı ({registeredUserId}) için {newWallet.Code} kodlu cüzdan başarıyla oluşturuldu!");
    }
}