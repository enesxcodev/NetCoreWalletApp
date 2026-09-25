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
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using WalletTransfer = Domain.Entities.Transaction;

namespace WalletApp.Tests.Application;

public class TransactionCreateHandlerTests
{
    [Fact]
    public async Task Handle_WhenSameIdempotencyKeyIsReplayed_ShouldReturnExistingTransactionWithoutTransfer()
    {
        var transactionId = Guid.NewGuid();
        var userId = Guid.NewGuid();
        var key = Guid.NewGuid().ToString("D");
        var command = new TransactionCreateCommand("WLT-RECEIVER", 100, "Test") { IdempotencyKey = key };
        var record = new IdempotencyRecord(userId, key, ComputeHash(userId, command), transactionId, 200, "{}",
            DateTime.UtcNow, DateTime.UtcNow.AddHours(24));

        var transactionRepository = new Mock<ITransaction>();
        var walletRepository = new Mock<IWalletRepository>();
        var idempotencyRepository = new Mock<IIdempotencyRepository>();
        var userContext = new Mock<IUserContext>();
        var logger = new Mock<ILogger<TransactionCreateHandler>>();
        var publisher = new Mock<IPublishEndpoint>();
        userContext.Setup(x => x.UserId).Returns(userId);
        idempotencyRepository.Setup(x => x.GetAsync(userId, key, It.IsAny<CancellationToken>())).ReturnsAsync(record);

        var handler = new TransactionCreateHandler(transactionRepository.Object, walletRepository.Object,
            idempotencyRepository.Object, userContext.Object, logger.Object, publisher.Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Data.Should().Be(transactionId);
        transactionRepository.Verify(x => x.AddAsync(It.IsAny<WalletTransfer>(), It.IsAny<CancellationToken>()), Times.Never);
        publisher.Verify(x => x.Publish(It.IsAny<MoneyTransferredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }

    [Fact]
    public async Task Handle_WhenIdempotencyKeyIsReusedWithDifferentPayload_ShouldReturnConflict()
    {
        var userId = Guid.NewGuid();
        var key = Guid.NewGuid().ToString("D");
        var command = new TransactionCreateCommand("WLT-RECEIVER", 200, "Yeni") { IdempotencyKey = key };
        var record = new IdempotencyRecord(userId, key, "DIFFERENT_HASH", Guid.NewGuid(), 200, "{}",
            DateTime.UtcNow, DateTime.UtcNow.AddHours(24));
        var idempotencyRepository = new Mock<IIdempotencyRepository>();
        var userContext = new Mock<IUserContext>();
        userContext.Setup(x => x.UserId).Returns(userId);
        idempotencyRepository.Setup(x => x.GetAsync(userId, key, It.IsAny<CancellationToken>())).ReturnsAsync(record);

        var handler = new TransactionCreateHandler(new Mock<ITransaction>().Object, new Mock<IWalletRepository>().Object,
            idempotencyRepository.Object, userContext.Object, new Mock<ILogger<TransactionCreateHandler>>().Object,
            new Mock<IPublishEndpoint>().Object);
        var result = await handler.Handle(command, CancellationToken.None);

        result.IsFailure.Should().BeTrue();
        ((int)result.Status).Should().Be(409);
    }

    [Fact]
    public async Task Handle_WhenValidRequest_ShouldExecuteTransferAndPublishEventSuccessfully()
    {
        // 1. Arrange bağımlıkları ekliyoz
        var mockTxRepo = new Mock<ITransaction>();
        var mockWalletRepo = new Mock<IWalletRepository>();
        var mockUserContext = new Mock<IUserContext>();
        var mockLogger = new Mock<ILogger<TransactionCreateHandler>>();
        var mockIdempotency = new Mock<IIdempotencyRepository>();
        var mockPublish = new Mock<IPublishEndpoint>();

        // veri setlerini hazırlıyoz
        var senderUserId = Guid.NewGuid();
        var senderWallet = new Wallet(userId: senderUserId, code: "WLT-SENDER", balance: 1000); 
        var receiverWallet = new Wallet(userId: Guid.NewGuid(), code: "WLT-RECEIVER", balance: 100); 

        var command = new TransactionCreateCommand(Amount: 300, WalletCode: "WLT-RECEIVER", Description: "elden borç")
        { IdempotencyKey = Guid.NewGuid().ToString("D") };

        // Mock Kurulumları
        mockUserContext.Setup(x => x.UserId).Returns(senderUserId);

        mockWalletRepo.Setup(r => r.GetByUserIdAsync(senderUserId, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(senderWallet);

        mockWalletRepo.Setup(r => r.GetByCodeAsync(command.WalletCode, It.IsAny<CancellationToken>()))
                      .ReturnsAsync(receiverWallet);

        var handler = new TransactionCreateHandler(
            mockTxRepo.Object, mockWalletRepo.Object, mockIdempotency.Object,
            mockUserContext.Object, mockLogger.Object, mockPublish.Object
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
        mockIdempotency.Verify(r => r.AddAsync(It.IsAny<IdempotencyRecord>(), It.IsAny<CancellationToken>()), Times.Once);

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
        var mockIdempotency = new Mock<IIdempotencyRepository>();
        var mockPublish = new Mock<IPublishEndpoint>();

        var senderUserId = Guid.NewGuid();
        var senderWallet = new Wallet(userId: senderUserId, code: "WLT-SENDER", balance: 500);

        var command = new TransactionCreateCommand(Amount: 100, WalletCode: "GEÇERSİZ-KOD", Description: "Test")
        { IdempotencyKey = Guid.NewGuid().ToString("D") };

        mockUserContext.Setup(x => x.UserId).Returns(senderUserId);
        mockWalletRepo.Setup(r => r.GetByUserIdAsync(senderUserId, It.IsAny<CancellationToken>())).ReturnsAsync(senderWallet);

        // 🎯 Alıcı cüzdan bulunamadığında NULL dönecek şekilde mock'luyoruz
        mockWalletRepo.Setup(r => r.GetByCodeAsync(command.WalletCode, It.IsAny<CancellationToken>())).ReturnsAsync((Wallet)null!);

        var handler = new TransactionCreateHandler(
            mockTxRepo.Object, mockWalletRepo.Object, mockIdempotency.Object,
            mockUserContext.Object, mockLogger.Object, mockPublish.Object
        );

        // 2. Act
        var result = await handler.Handle(command, CancellationToken.None);

        // 3. Assert
        result.IsSuccess.Should().BeFalse();

        // Gönderenin parasına dokunulmamış olmalı (Güvenlik)
        senderWallet.Balance.Should().Be(500);

        // SQL'e ve RabbitMQ'ya ASLA bir şey gitmemeli!
        mockIdempotency.Verify(r => r.AddAsync(It.IsAny<IdempotencyRecord>(), It.IsAny<CancellationToken>()), Times.Never);
        mockPublish.Verify(p => p.Publish(It.IsAny<MoneyTransferredEvent>(), It.IsAny<CancellationToken>()), Times.Never);
    }
    private static string ComputeHash(Guid userId, TransactionCreateCommand request)
    {
        var canonicalValue = string.Join('|', userId.ToString("N"), request.WalletCode.Trim().ToUpperInvariant(),
            request.Amount.ToString("0.############################", CultureInfo.InvariantCulture),
            (request.Description ?? "transfer").Trim());
        return Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(canonicalValue)));
    }
}
