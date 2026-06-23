using Application.Features.Transaction.Commands;
using Application.Features.Transaction.Handlers;
using Application.Contracts;
using Application.Contracts.Events;
using Domain.Common;
using Domain.Entities;
using FluentAssertions;
using MassTransit;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;
using WalletTransfer = Domain.Entities.Transaction;

namespace WalletApp.Tests.Application;

public class TransactionCreateHandlerTests
{
    [Fact]
    public async Task Handle_WhenValidRequest_ShouldExecuteTransferAndPublishEventSuccessfully()
    {
        // 1. Arrange bağımlıkları ekliyoz
        var mockTxRepo = new Mock<ITransaction>();
        var mockWalletRepo = new Mock<IWalletRepository>();
        var mockUserContext = new Mock<IUserContext>();
        var mockLogger = new Mock<ILogger<TransactionCreateHandler>>();
        var mockUow = new Mock<IUnitOfWork>();
        var mockCache = new Mock<IDistributedCache>();
        var mockPublish = new Mock<IPublishEndpoint>();

        // veri setlerini hazırlıyoz
        var senderUserId = Guid.NewGuid();
        var senderWallet = new Wallet(userId: senderUserId, code: "WLT-SENDER", balance: 1000); 
        var receiverWallet = new Wallet(userId: Guid.NewGuid(), code: "WLT-RECEIVER", balance: 100); 

        var command = new TransactionCreateCommand(Amount: 300, WalletCode: "WLT-RECEIVER", Description: "elden borç");

        // Mock Kurulumları
        mockUserContext.Setup(x => x.UserId).Returns(senderUserId);

        mockWalletRepo.Setup(r => r.GetByUserIdAsync(senderUserId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(senderWallet);

        mockWalletRepo.Setup(r => r.GetByCodeAsync(command.WalletCode, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(receiverWallet);

        var handler = new TransactionCreateHandler(
            mockTxRepo.Object, mockWalletRepo.Object, mockUserContext.Object,
            mockLogger.Object, mockUow.Object, mockCache.Object, mockPublish.Object
        );

        // 2. Act 
        var result = await handler.Handle(command, CancellationToken.None);

        // 3. Assert 
        result.IsSuccess.Should().BeTrue();

        // 🚀 DOMAIN KONTROLLERİ: Bakiyeler doğru değişti mi?
        senderWallet.Balance.Should().Be(700);   // 1000 - 300 = 700
        receiverWallet.Balance.Should().Be(400); // 100 + 300 = 400

        // 🚀 SQL & TRANSACTION KONTROLÜ: Tabloya eklendi mi ve veritabanına kaydedildi mi kontolleri
        mockTxRepo.Verify(r => r.AddAsync(It.IsAny<WalletTransfer>()), Times.Once);
        mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);

        // 🚀 CACHE KONTROLÜ: Redis'teki iki cache prefixi de uçurulmaya çalışıldı mı
        // (Burada RemoveByPrefixAsync extension metodunu doğrudan mock'layamayacağımız için         

        // 🚀 MASSTRANSIT / RABBITMQ KONTROLÜ: En kritik yer! Mesaj kuyruğa fırlatıldı mı?
        mockPublish.Verify(p => p.Publish(
            It.Is<MoneyTransferredEvent>(e =>
                e.SenderWalletCode == "WLT-SENDER" &&
                e.ReceiverWalletCode == "WLT-RECEIVER" &&
                e.Amount == 300),
            It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact]
    public async Task Handle_WhenReceiverWalletNotFound_ShouldReturnReceiveWalletNotFoundFailure()
    {
        // 1. Arrange (Alıcı cüzdanın bulunamadığı senaryo)
        var mockTxRepo = new Mock<ITransaction>();
        var mockWalletRepo = new Mock<IWalletRepository>();
        var mockUserContext = new Mock<IUserContext>();
        var mockLogger = new Mock<ILogger<TransactionCreateHandler>>();
        var mockUow = new Mock<IUnitOfWork>();
        var mockCache = new Mock<IDistributedCache>();
        var mockPublish = new Mock<IPublishEndpoint>();

        var senderUserId = Guid.NewGuid();
        var senderWallet = new Wallet(userId: senderUserId, code: "WLT-SENDER", balance: 500);

        var command = new TransactionCreateCommand(Amount: 100, WalletCode: "GEÇERSİZ-KOD", Description: "Test");

        mockUserContext.Setup(x => x.UserId).Returns(senderUserId);
        mockWalletRepo.Setup(r => r.GetByUserIdAsync(senderUserId, It.IsAny<CancellationToken>())).ReturnsAsync(senderWallet);

        // 🎯 Alıcı cüzdan bulunamadığında NULL dönecek şekilde mock'luyoruz
        mockWalletRepo.Setup(r => r.GetByCodeAsync(command.WalletCode, It.IsAny<CancellationToken>())).ReturnsAsync((Wallet)null!);

        var handler = new TransactionCreateHandler(
            mockTxRepo.Object, mockWalletRepo.Object, mockUserContext.Object,
            mockLogger.Object, mockUow.Object, mockCache.Object, mockPublish.Object
        );

        // 2. Act
        var result = await handler.Handle(command, CancellationToken.None);

        // 3. Assert
        result.IsSuccess.Should().BeFalse();

        // Gönderenin parasına dokunulmamış olmalı (Güvenlik)
        senderWallet.Balance.Should().Be(500);

        // SQL'e ve RabbitMQ'ya ASLA bir şey gitmemeli!
        mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        mockPublish.Verify(p => p.Publish(It.IsAny<MoneyTransferredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
}