using Application.Contracts;
using Application.Features.Wallet.Commands;
using Application.Features.Wallet.Handlers;
using Microsoft.Extensions.Logging;
using Moq;
using FluentAssertions;
using System;
using System.Collections.Generic;
using System.Text;
using Domain.Entities;

namespace Test
{
    public class DepositMoney_Handler_Test
    {
        [Fact]
        public async Task Handle_WhenWalletExists_ShouldIncreaseBalanceAndSaveSuccessfully()
        {
            // 1. Arrange (Hazırlık)
            var mockWalletRepo = new Mock<IWalletRepository>();
            var mockUserContext = new Mock<IUserContext>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<DepositMoneyHandler>>();

            var userId = Guid.NewGuid();
            var command = new DepositMoneyCommand(250); // 250 TL yatırmak istiyoruz

            // Başlangıçta 500 TL bakiyesi olan sahte bir cüzdan nesnesi üretiyoruz
            var fakeWallet = new Wallet(userId: userId,code: "WLT-123",balance: 500);

            // Mock 1: UserContext çağrıldığında testteki userId'yi dönsün
            mockUserContext.Setup(x => x.UserId).Returns(userId);

            // Mock 2: Repo'dan bu kullanıcıya ait cüzdan istendiğinde bizim fake cüzdanı dönsün
            mockWalletRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync(fakeWallet);

            var handler = new DepositMoneyHandler(mockWalletRepo.Object, mockUserContext.Object, mockUow.Object, mockLogger.Object);

            // 2. Act (Çalıştırma)
            var result = await handler.Handle(command, CancellationToken.None);

            // 3. Assert (Doğrulama)
            result.IsSuccess.Should().BeTrue(); // Handler başarılı dönmeli

            // 🚀 DOMAIN KONTROLÜ: 500 + 250 = 750 TL olmuş mu?
            fakeWallet.Balance.Should().Be(750);

            // 🚀 VERİTABANI KONTROLÜ: Update ve SaveChanges metotları tetiklendi mi?
            mockWalletRepo.Verify(r => r.Update(fakeWallet), Times.Once);
            mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public async Task Handle_WhenWalletNotFound_ShouldReturnFailureResult()
        {
            // 1. Arrange (Cüzdanın Bulunamama Senaryosu)
            var mockWalletRepo = new Mock<IWalletRepository>();
            var mockUserContext = new Mock<IUserContext>();
            var mockUow = new Mock<IUnitOfWork>();
            var mockLogger = new Mock<ILogger<DepositMoneyHandler>>();

            var userId = Guid.NewGuid();
            var command = new DepositMoneyCommand(100);

            mockUserContext.Setup(x => x.UserId).Returns(userId);

            // Cüzdan veritabanında yoksa NULL dönecek şekilde ayarlıyoruz 
            mockWalletRepo.Setup(r => r.GetByUserIdAsync(userId, It.IsAny<CancellationToken>()))
                          .ReturnsAsync((Wallet)null!);

            var handler = new DepositMoneyHandler(mockWalletRepo.Object, mockUserContext.Object, mockUow.Object, mockLogger.Object);

            // 2. Act
            var result = await handler.Handle(command, CancellationToken.None);

            // 3. Assert
            result.IsSuccess.Should().BeFalse(); // Başarısız olmalı
            result.Error.Should().Be("Cüzdan bulunamadı"); // Hata mesajı eşleşmeli

            // Cüzdan yoksa Update ve SaveChanges ASLA tetiklenmemeli! (Güvenlik Kontrolü)
            mockWalletRepo.Verify(r => r.Update(It.IsAny<Wallet>()), Times.Never);
            mockUow.Verify(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Never);
        }
    }
}
