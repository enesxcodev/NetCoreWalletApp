using Application.Common;
using Application.Common.Constant;
using Application.Contracts;
using Application.Contracts.Events; 
using Application.Features.Transaction.Commands;
using Domain.Common;
using MassTransit; // 🎯 MassTransit'i ekledik
using MediatR;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using WalletTransfer = Domain.Entities.Transaction;

namespace Application.Features.Transaction.Handlers
{
    public class TransactionCreateHandler(
        ITransaction transactionRepository,
        IWalletRepository walletRepository,
        IUserContext userContext,
        ILogger<TransactionCreateHandler> logger,
        IUnitOfWork unitOfwork,
        IDistributedCache cache,
        IPublishEndpoint publishEndpoint // 🎯 auditService gitti, yerine kuyruk fırlatıcısı geldi!
        ) : IRequestHandler<TransactionCreateCommand, Result>
    {
        public async Task<Result> Handle(TransactionCreateCommand request, CancellationToken cancellationToken)
        {
            var senderId = userContext.UserId;

            // Önce gönderenin wallet'ını çekelim
            var senderWallet = await walletRepository.GetByUserIdAsync(senderId, cancellationToken);
            if (senderWallet is null)
            {
                logger.LogWarning("{senderId} nolu wallet bulunamamıştır", senderId);
                return Result.Failure(Messages.Wallet.WalletNotFound);
            }

            // Alıcının cüzdanını çek
            var receiverWallet = await walletRepository.GetByCodeAsync(request.WalletCode, cancellationToken);
            if (receiverWallet is null)
            {
                logger.LogWarning("{WalletCode} nolu wallet bulunamamıştır", request.WalletCode);
                return Result.Failure(Messages.Wallet.ReceiveWalletNotFound);
            }

            // Kontroller tamam şimdi cüzdanlara işle
            senderWallet.Withdraw(request.Amount);
            receiverWallet.Deposit(request.Amount);

            var transfer = new WalletTransfer(senderWallet.Id, receiverWallet.Id, request.Amount, request.Description!, TransferType.Out);

            // Güncelleme işlemlerini bas            
            await transactionRepository.AddAsync(transfer);

            // 🎯 SQL'e kaydet
            await unitOfwork.SaveChangesAsync(cancellationToken);

            // 🎯 Kayıt başarılı olduktan sonra Redis cache'lerini uçuruyoruz
            string senderCachePrefix = $"tx_history:{senderWallet.Id}";
            string receiverCachePrefix = $"tx_history:{receiverWallet.Id}";

            await cache.RemoveByPrefixAsync(senderCachePrefix);
            await cache.RemoveByPrefixAsync(receiverCachePrefix);

            // 🚀 rabbitmq ya datanın mongodb ya yazmasının eventi
            var transferEvent = new MoneyTransferredEvent(
                transfer.Id,
                senderWallet.Id,
                senderWallet.Code,
                receiverWallet.Id,
                receiverWallet.Code,
                request.Amount,
                request.Description ?? "transfer"
            );

            await publishEndpoint.Publish(transferEvent, cancellationToken);
            logger.LogInformation("MoneyTransferredEvent başarıyla RabbitMQ'ya fırlatıldı.");
            logger.LogInformation("Para Transferi:{senderId} nolu kullanıcı {Code} nolu cüzdana {Amount} miktar göndermiştir", senderId, receiverWallet.Code, request.Amount);
            return Result.Success();
        }
    }
}